using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ExpressionInterpreterService
{
    public async Task<Dictionary<string, object>> GetExpressionCommands(string reply)
    {
        // Updated system instruction for the second chatbot
        var systemInstruction = "You are an assistant that analyzes a chatbot's reply to determine the facial and body expressions of a game character. The reply may contain expressions enclosed in asterisks (*), such as *shaking head slightly*. Your task is to identify these expressions and convert them into specific animation commands for the character's animation system. The animation system has the following parameters: neckUp_Down, neckLeft_Right, neckTiltLeft_Right, headUp_Down, headLeft_Right, headTiltLeft_Right, jawOpen_Close, jawForward_Back, jawLeft_Right, mouthLeft_Right, mouthUp_Down, mouthNarrow_Pucker, tongueOut, tongueCurl (0 to 1), tongueUp_Down, tongueLeft_Right, tongueWide_Narrow, leftMouthSmile_Frown, rightMouthSmile_Frown, leftLowerLipUp_Down, rightLowerLipUp_Down, leftUpperLipUp_Down, rightUpperLipUp_Down, leftCheekPuff_Squint, rightCheekPuff_Squint, noseSneer (0 to 1), leftEyeOpen_Close, rightEyeOpen_Close, leftEyeUp_Down, rightEyeUp_Down, leftEyeIn_Out, rightEyeIn_Out, browsIn (0 to 1), leftBrowUp_Down, rightBrowUp_Down, midBrowUp_Down, leftGrasp (0 to 1), rightGrasp (0 to 1), leftPeace (0 to 1), rightPeace (0 to 1), leftRude (0 to 1), rightRude (0 to 1), leftPoint (0 to 1), rightPoint (0 to 1). Each parameter ranges from -1 to 1 unless specified otherwise. For the identified expressions in the reply, provide a JSON object where the keys are the parameter names and the values are either a float for static values or an object with 'type': 'oscillate', 'min': float, 'max': float, 'frequency': float for oscillating values. If there are multiple expressions, combine the commands appropriately. If there are no expressions, return an empty JSON object.";

        var messagesArray = new JArray
        {
            new JObject { { "role", "system" }, { "content", systemInstruction } },
            new JObject { { "role", "user" }, { "content", reply } }
        };

        JObject payload = new JObject
        {
            { "messages", messagesArray },
            { "model", "deepseek-chat" },
            { "frequency_penalty", 0 },
            { "max_tokens", 2048 },
            { "presence_penalty", 0 },
            { "response_format", new JObject(new JProperty("type", "json_object")) },
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
            //Debug.Log("Response: " + responseString);
            string chatbotReply = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();

            JObject commandsJson = JObject.Parse(chatbotReply);
            //Debug.Log("Parsed commands: " + commandsJson.ToString());
            var result = new Dictionary<string, object>();
            foreach (var property in commandsJson.Properties())
            {
                if (property.Value.Type == JTokenType.Object)
                {
                    var oscillate = new OscillateCommand
                    {
                        min = property.Value["min"].Value<float>(),
                        max = property.Value["max"].Value<float>(),
                        frequency = property.Value["frequency"].Value<float>()
                    };
                    result[property.Name] = oscillate;
                }
                else
                {
                    result[property.Name] = property.Value.Value<float>();
                }
            }
            Debug.Log("Received commands: " + JsonUtility.ToJson(result));
            return result;
        }
    }
}