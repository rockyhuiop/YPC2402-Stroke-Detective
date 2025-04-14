using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Xml.Linq;

// ChatbotService handles sending the multi-round conversation HTTP request.
public class ChatbotService
{
    // This function sends the conversation history to the API and returns the chatbot's reply.
    public async Task<string> GetChatbotReply(List<(string Role, string Content)> conversationHistory, string description)
    {
        // System instruction message for the assistant.
        var assistantInstruction = "You are portraying an elderly man who is suffering from a stroke. " +
        "Your speech is very limited and marked by short, halting phrases; you struggle to articulate complex thoughts. " +
        "You speak in brief, fragmented sentences and often pause, as if you're having difficulty forming words. " +
        "The user is trying to determine if you have a stroke by asking questions, so respond using very few words and incomplete phrases. " +
        "Maintain a reserved and somber tone throughout your conversation.";

        // Ensure the conversation history exists.
        if (conversationHistory == null)
        {
            conversationHistory = new List<(string Role, string Content)>();
        }

        // Add the system message at the beginning if it hasn't been added yet.
        if (conversationHistory.Count == 0 || conversationHistory[0].Role != "system")
        {
            conversationHistory.Insert(0, ("system", description));
            //conversationHistory.Insert(0, ("system", assistantInstruction));
        }

        // Build the messages array for the JSON payload.
        var messagesArray = new JArray();
        foreach (var (role, content) in conversationHistory)
        {
            var messageObject = new JObject
            {
                { "role", role },
                { "content", content }
            };
            messagesArray.Add(messageObject);
        }

        // Construct the JSON payload with all required properties.
        JObject payload = new JObject
        {
            { "messages", messagesArray },
            { "model", "deepseek-chat" },
            { "frequency_penalty", 0 },
            { "max_tokens", 2048 },
            { "presence_penalty", 0 },
            { "response_format", new JObject(new JProperty("type", "text")) },
            { "stop", null },
            { "stream", false },
            { "stream_options", null },
            { "temperature", 1 },
            { "top_p", 1 },
            { "tools", null },
            { "tool_choice", "none" },
            { "logprobs", false },
            { "top_logprobs", null }
        };

        // Create an HttpClient instance and prepare the request.
        using (var client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.deepseek.com/chat/completions");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer sk-8d933c54c1584f09af8bee7c0dfc4a0f");

            // Specify the encoding as UTF-8 to ensure proper handling of the payload.
            var contentHttp = new StringContent(payload.ToString(), System.Text.Encoding.UTF8, "application/json");
            Debug.Log("Request payload: " + payload.ToString());
            request.Content = contentHttp;

            // Send the request and ensure a successful response.
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Read and parse the response.
            var responseString = await response.Content.ReadAsStringAsync();
            JObject jsonResponse = JObject.Parse(responseString);

            // Extract the chatbot reply from the first available response choice.
            string chatbotReply = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();

            return chatbotReply;
        }
    }
}

public class ChatbotManager : MonoBehaviour
{
    [SerializeField] TMP_Text userText;
    [SerializeField] TMP_Text chatbotText;
    [SerializeField] StrokeDetectiveNPCData NPCData;
    [SerializeField] CognitiveSpeech cognitiveSpeech;
    [SerializeField] Button isStrokeBtn;
    [SerializeField] Button isNotStrokeBtn;

    private List<(string Role, string Content)> conversationHistory = new List<(string Role, string Content)>();
    private ChatbotService chatbotService = new ChatbotService();
    private ExpressionInterpreterService expressionInterpreterService = new ExpressionInterpreterService(); // Add second chatbot service
    private ExpressionControl expressionControl; // Reference to ExpressionControl

    private bool isTalking = false;
    private bool isEndofChat = false;
    public bool end = false;
    public int ChatToken = 5;
    private IEnumerator LoadingC;
    [SerializeField] GameObject rootGameObject, correctSign, wrongSign;

    private void Start()
    {
        NPCData = GetComponent<Transform>().parent.GetComponent<NPC>().NPCData;
        rootGameObject = GetComponent<Transform>().parent.gameObject;
        expressionControl = rootGameObject.GetComponent<ExpressionControl>(); // Get ExpressionControl component
        userText = GameObject.FindObjectOfType<PlayerSubtitleController>().subtitleTextMesh;
        isStrokeBtn.onClick.AddListener(() =>
        {
            isNotStrokeBtn.interactable = false;
            GameManager.instance.DetermineStroke(true, rootGameObject, correctSign, wrongSign);
            end = true;
        });
        isNotStrokeBtn.onClick.AddListener(() =>
        {
            isStrokeBtn.interactable = false;
            GameManager.instance.DetermineStroke(false, rootGameObject, correctSign, wrongSign);
            end = true;
        });
        cognitiveSpeech = GameObject.FindObjectOfType<CognitiveSpeech>();
    }

    public async void StartChat()
    {
        if (NPCData.symptoms != "cannot speak")
        {
            await ChatLoop();
        }
        if (isEndofChat)
        {
            Debug.Log("End of Chat");
        }
    }

    private async Task ChatLoop()
    {
        while (!end)
        {
            if (isTalking)
            {
                await Task.Delay(100);
                continue;
            }
            string userInput = await cognitiveSpeech.RecognizeSpeechAsync();
            userText.SetText(userInput);

            if (string.IsNullOrEmpty(userInput))
            {
                Debug.Log("No valid speech detected. Please try again.");
                continue;
            }

            Debug.Log("User said: " + userInput);
            conversationHistory.Add(("user", userInput));
            LoadingC = LoadingCoroutine();
            StartCoroutine(LoadingC);
            string chatbotReply = await chatbotService.GetChatbotReply(conversationHistory, NPCData.NPCDescription);
            StopCoroutine(LoadingC);
            Debug.Log("Chatbot reply: " + chatbotReply);

            chatbotText.text = RemoveExpressionCommands(chatbotReply);

            var commands = await expressionInterpreterService.GetExpressionCommands(chatbotReply);
            
            expressionControl.ApplyExpressionCommands(commands);

            conversationHistory.Add(("assistant", chatbotReply));
            isTalking = true;
            ChatToken--;
            if (ChatToken <= 0)
            {
                isEndofChat = true;
                break;
            }

            await cognitiveSpeech.SynthesizeSpeech(chatbotText.text, OnAudioPlaybackFinished);
            await Task.Delay(500);
        }
    }

    // Existing LoadingCoroutine and OnAudioPlaybackFinished methods remain unchanged
    IEnumerator LoadingCoroutine()
    {
        string loading = ".";
        while (true)
        {
            chatbotText.SetText(loading);
            yield return new WaitForSeconds(1);
            if (loading != "...")
            {
                loading += ".";
            }
            else
            {
                loading = ".";
            }
        }
    }

    private void OnAudioPlaybackFinished()
    {
        isTalking = false;
    }

    // Existing ExtractExpressions and RemoveExpressionCommands methods remain unchanged
    
    public List<string> ExtractExpressions(string text)
    {
        Regex regex = new Regex(@"\*(.*?)\*|\((.*?)\)");
        MatchCollection matches = regex.Matches(text);
        List<string> extractedExpressions = new List<string>();
        foreach (Match match in matches)
        {
            string extracted = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
            extractedExpressions.Add(extracted.Trim());
            Debug.Log("Extracted expression: " + extracted.Trim());
        }
        return extractedExpressions;
    }

    public string RemoveExpressionCommands(string input)
    {
        string cleanedText = Regex.Replace(input, @"\*(.*?)\*|\((.*?)\)", string.Empty);
        return cleanedText.Trim();
    }
}