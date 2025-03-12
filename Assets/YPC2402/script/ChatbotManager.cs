using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// ChatbotService handles sending the multi-round conversation HTTP request.
public class ChatbotService
{
    // This function sends the conversation history to the API and returns the chatbot's reply.
    public async Task<string> GetChatbotReply(List<(string Role, string Content)> conversationHistory)
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
            conversationHistory.Insert(0, ("system", assistantInstruction));
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

    // Reference to the CognitiveSpeech component (for text-to-speech)
    [SerializeField] CognitiveSpeech cognitiveSpeech;

    [SerializeField] Button isStrokeBtn;
    [SerializeField] Button isNotStrokeBtn;

    private List<(string Role, string Content)> conversationHistory = new List<(string Role, string Content)>();

    private ChatbotService chatbotService = new ChatbotService();

    private bool isTalking = false;
    private bool isEndofChat = false;

    public int ChatToken = 5;

    [SerializeField] GameObject rootGameObject, correctSign, wrongSign;
    

    private void Start()
    {
        isStrokeBtn.onClick.AddListener(() => {
            isNotStrokeBtn.interactable = false;
            GameManager.instance.DetermineStroke(true, rootGameObject, correctSign, wrongSign);
        });

        isNotStrokeBtn.onClick.AddListener(() => {
            isStrokeBtn.interactable = false;
            GameManager.instance.DetermineStroke(false, rootGameObject, correctSign, wrongSign);
        });

    }

    public async void StartChat()
    {
        await ChatLoop();
        if(isEndofChat){
            Debug.Log("End of Chat");
        }
    }

    /// <summary>
    /// Continuously listens for user input, sends the recognized text to the chatbot via DeepSeek API,
    /// and then uses TTS to speak the chatbot's reply.
    /// </summary>
    private async Task ChatLoop()
    {
        while (true)
        {
            if (isTalking)
            {
                await Task.Delay(100);
                continue;
            }

            //chatbotText.text = "Please say something to chat with the bot...";
            string userInput = await cognitiveSpeech.RecognizeSpeechAsync();
            userText.text = userInput;

            if (string.IsNullOrEmpty(userInput))
            {
                Debug.Log("No valid speech detected. Please try again.");
                continue;
            }

            Debug.Log("User said: " + userInput);
            conversationHistory.Add(("user", userInput));

            string chatbotReply = await chatbotService.GetChatbotReply(conversationHistory);
            Debug.Log("Chatbot reply: " + chatbotReply);

            // Extract expressions from the chatbot reply.
            chatbotText.text = RemoveExpressionCommands(chatbotReply);
            List<string> expressions =  ExtractExpressions(chatbotReply);

            // Log the extracted expressions.
            foreach (var expression in expressions)
            {
                Debug.Log("Extracted Expression: " + expression);
            }
            conversationHistory.Add(("assistant", chatbotReply));
            isTalking = true;
            ChatToken--;
            if(ChatToken <= 0){
                isEndofChat = true;
                break;
            }

            // Use the CognitiveSpeech component to synthesize and play the reply.
            await cognitiveSpeech.SynthesizeSpeech(chatbotText.text, OnAudioPlaybackFinished);

            // Optionally, add a delay or exit condition here.
            await Task.Delay(500); // slight pause between interactions
        }
    }

    private void OnAudioPlaybackFinished()
    {
        isTalking = false;
    }


    /// <summary>
    /// Extracts messages enclosed in asterisks (*) from a given text.
    /// </summary>
    /// <param name="text">The input text containing expressions.</param>
    /// <returns>A list of strings containing the extracted expressions.</returns>
    public List<string> ExtractExpressions(string text)
    {
        // Define a regular expression pattern to find text that is enclosed in asterisks.
        // The pattern \*(.*?)\* uses a non-greedy match to capture content between asterisks.
        Regex regex = new Regex(@"\*(.*?)\*");
        MatchCollection matches = regex.Matches(text);

        List<string> extractedExpressions = new List<string>();

        foreach (Match match in matches)
        {
            // match.Groups[1] contains the content between the asterisks.
            if (match.Groups.Count > 1)
            {
                extractedExpressions.Add(match.Groups[1].Value.Trim());
            }
        }

        return extractedExpressions;
    }

    public string RemoveExpressionCommands(string input)
    {
        // Use Regex.Replace to remove all occurrences of text enclosed in asterisks.
        // The pattern \*.*?\* matches any substring starting and ending with an asterisk.
        string cleanedText = Regex.Replace(input, @"\*.*?\*", string.Empty);
        
        // Optionally, you can trim extra white spaces that result from the removal.
        return cleanedText.Trim();
    }
}
