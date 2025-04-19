using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class DetermineChatbotService
{
    private readonly string systemInstruction = "You are an assistant that classifies user inputs into categories related to stroke diagnosis using the F.A.S.T. method. " +
        "The categories are: 'face' (if the user is asking about the face or smiling), 'arms' (if the user is asking about raising arms), " +
        "'speech' (if the user is asking about speech or repeating a phrase), or 'other' (if the input is not related to these). " +
        "Respond with only the category name.";

    public async Task<string> GetCategory(string userInput)
    {
        var messages = new List<(string Role, string Content)>
        {
            ("system", systemInstruction),
            ("user", userInput)
        };

        var messagesArray = new JArray();
        foreach (var (role, content) in messages)
        {
            var messageObject = new JObject
            {
                { "role", role },
                { "content", content }
            };
            messagesArray.Add(messageObject);
        }

        JObject payload = new JObject
        {
            { "messages", messagesArray },
            { "model", "deepseek-chat" },
            { "frequency_penalty", 0 },
            { "max_tokens", 10 }, // Short response expected
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

        using (var client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.deepseek.com/chat/completions");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer sk-8d933c54c1584f09af8bee7c0dfc4a0f");
            request.Content = new StringContent(payload.ToString(), System.Text.Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            JObject jsonResponse = JObject.Parse(responseString);
            string category = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString().Trim().ToLower();

            // Validate response
            if (category == "face" || category == "arms" || category == "speech" || category == "other")
            {
                return category;
            }
            else
            {
                Debug.LogWarning($"Unexpected category response: {category}. Defaulting to 'other'.");
                return "other";
            }
        }
    }
}