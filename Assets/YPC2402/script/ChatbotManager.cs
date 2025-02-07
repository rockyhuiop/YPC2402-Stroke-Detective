using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;

// ChatbotService handles sending the multi-round conversation HTTP request.
public class ChatbotService
{
    // This function sends the conversation history to the API and returns the chatbot's reply.
    public async Task<string> GetChatbotReply(List<(string Role, string Content)> conversationHistory)
    {
        // System instruction message for the assistant.
        var assistantInstruction = "You are an old man with a stroke currently. " +
            "Now, the user is trying to find out whether you have a stroke or not. " +
            "The user is going to ask some questions to you to see if there are any hints of stroke. " +
            "You may reply correspondingly to the user's question. ";

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

    private List<(string Role, string Content)> conversationHistory = new List<(string Role, string Content)>();

    private ChatbotService chatbotService = new ChatbotService();
    private ExpressionExtractor expressionExtractor = new ExpressionExtractor();


    public async void StartChat()
    {
        await ChatLoop();
    }

    /// <summary>
    /// Continuously listens for user input, sends the recognized text to the chatbot via DeepSeek API,
    /// and then uses TTS to speak the chatbot's reply.
    /// </summary>
    private async Task ChatLoop()
    {
        while (true)
        {
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
            chatbotText.text = expressionExtractor.RemoveExpressionCommands(chatbotReply);
            List<string> expressions =  expressionExtractor.ExtractExpressions(chatbotReply);

            // Log the extracted expressions.
            foreach (var expression in expressions)
            {
                Debug.Log("Extracted Expression: " + expression);
            }
            conversationHistory.Add(("assistant", chatbotReply));

            // Use the CognitiveSpeech component to synthesize and play the reply.
            await cognitiveSpeech.SynthesizeSpeech(chatbotText.text);

            // Optionally, add a delay or exit condition here.
            await Task.Delay(500); // slight pause between interactions
        }
    }
}
