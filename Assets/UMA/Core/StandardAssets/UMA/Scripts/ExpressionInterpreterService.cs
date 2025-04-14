using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class ExpressionInterpreterService
{
    public async Task<Dictionary<string, object>> GetExpressionCommands(string reply)
    {
        var systemInstruction = @"You are an assistant that analyzes a chatbot's reply to determine the facial and body expressions of a game character. The reply may contain expressions enclosed in asterisks (*), such as *shaking head slightly* or *lifts arm confidently*. Your task is to identify these expressions and convert them into specific animation commands for the character's animation system.

The animation system has the following parameters: neckUp_Down, neckLeft_Right, neckTiltLeft_Right, headUp_Down, headLeft_Right, headTiltLeft_Right, jawOpen_Close, jawForward_Back, jawLeft_Right, mouthLeft_Right, mouthUp_Down, mouthNarrow_Pucker, tongueOut, tongueCurl (0 to 1), tongueUp_Down, tongueLeft_Right, tongueWide_Narrow, leftMouthSmile_Frown, rightMouthSmile_Frown, leftLowerLipUp_Down, rightLowerLipUp_Down, leftUpperLipUp_Down, rightUpperLipUp_Down, leftCheekPuff_Squint, rightCheekPuff_Squint, noseSneer (0 to 1), leftEyeOpen_Close, rightEyeOpen_Close, leftEyeUp_Down, rightEyeUp_Down, leftEyeIn_Out, rightEyeIn_Out, browsIn (0 to 1), leftBrowUp_Down, rightBrowUp_Down, midBrowUp_Down, leftGrasp (0 to 1), rightGrasp (0 to 1), leftPeace (0 to 1), rightPeace (0 to 1), leftRude (0 to 1), rightRude (0 to 1), leftPoint (0 to 1), rightPoint (0 to 1). Each parameter ranges from -1 to 1 unless specified otherwise.

For expressions that can be achieved by setting or oscillating these parameters, provide a JSON object with the parameter names as keys and either a float for static values or an object with 'type': 'oscillate', 'min': float, 'max': float, 'frequency': float for oscillating values.

For body animations, only include an 'animation_trigger' key when the expression explicitly involves lifting arms. Determine if the arm-lifting action is fully successful or only partially successful based on the description in the reply. If the action is fully successful (e.g., *lifts arm confidently*), use 'animation_trigger': 'lift_arm_full'. If the action is only partially successful or unsuccessful (e.g., *tries to lift arm but fails*), use 'animation_trigger': 'lift_arm_half'. For all other body animations (e.g., *waves*, *bows*, *shrugs*), do not include an 'animation_trigger'; instead, represent them using the available parameters if applicable (e.g., use leftGrasp, rightGrasp for pointing gestures) or omit them if they cannot be represented.

If there are multiple expressions or actions, combine the commands appropriately. If there are no expressions or actions, return an empty JSON object.";

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
            string chatbotReply = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();
            Debug.Log("Chatbot reply: " + chatbotReply);
            JObject commandsJson = JObject.Parse(chatbotReply);
            var result = new Dictionary<string, object>();
            foreach (var property in commandsJson.Properties())
            {
                if (property.Name == "animation_trigger" && property.Value.Type == JTokenType.String)
                {
                    result["animation_trigger"] = property.Value.Value<string>();
                }
                else if (property.Value.Type == JTokenType.Object)
                {
                    var oscillate = new OscillateCommand
                    {
                        min = property.Value["min"].Value<float>(),
                        max = property.Value["max"].Value<float>(),
                        frequency = property.Value["frequency"].Value<float>()
                    };
                    result[property.Name] = oscillate;
                }
                else if (property.Value.Type == JTokenType.Float || property.Value.Type == JTokenType.Integer)
                {
                    result[property.Name] = property.Value.Value<float>();
                }
                else
                {
                    Debug.LogWarning($"Unsupported value type for {property.Name}: {property.Value.Type}");
                }
            }
            Debug.Log("Received commands: " + JsonUtility.ToJson(result));
            return result;
        }
    }
}