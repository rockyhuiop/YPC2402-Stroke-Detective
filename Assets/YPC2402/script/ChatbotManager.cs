using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Azure;  
using Azure.AI.OpenAI;  
using Azure.Identity;  
using OpenAI.Chat;
using System.Collections.Generic;
using TMPro;
public class ChatbotManager : MonoBehaviour
{
    [SerializeField] TMP_Text userText;
    [SerializeField] TMP_Text chatbotText;

    // Reference to the CognitiveSpeech component (for text-to-speech)
    [SerializeField] CognitiveSpeech cognitiveSpeech;


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

            string chatbotReply = await GetChatbotReply(userInput);
            chatbotText.text = chatbotReply;
            Debug.Log("Chatbot reply: " + chatbotReply);

            // Use the CognitiveSpeech component to synthesize and play the reply.
            await cognitiveSpeech.SynthesizeSpeech(chatbotReply);

            // Optionally, add a delay or exit condition here.
            await Task.Delay(500); // slight pause between interactions
        }
    }


    /// <summary>
    /// Sends the recognized user input as a JSON payload to the DeepSeek chatbot API,
    /// and returns the chatbot's reply.
    /// </summary>
    /// <param name="userInput">User's recognized speech in text form.</param>
    /// <returns>Chatbot's reply text.</returns>
    private async Task<string> GetChatbotReply(string userInput)
    {
        // 從環境變數中擷取 OpenAI 端點
        var endpoint = "https://ai-11551720128698ai472434261403.openai.azure.com/";  
        
        var key = "FtAI11nvUy2wiLDvePosuHuVg3bUOujpphGHGhVApag4kRUI7Q1IJQQJ99ALACHYHv6XJ3w3AAAAACOGjqAO";
      
        AzureKeyCredential credential = new AzureKeyCredential(key); 

        // 將 AzureOpenAIClient 初始化
        AzureOpenAIClient azureClient = new(new Uri(endpoint), credential); 

        // 使用指定的部署名稱，將 ChatClient 初始化
        ChatClient chatClient = azureClient.GetChatClient("DeepSeek-R1");  
        
        // 建立聊天訊息清單
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are an old man with a stroke currently.  Now, the user is trying to find out whether you have a stroke or not. the user is going to ask some question to you to see if any hints of stroke. you may reply correspondingly to the user's question. before the final output, add the indication word \"MyAns:\""),
            new UserChatMessage(userInput)
        };

        
        // 建立聊天完成選項
        var options = new ChatCompletionOptions  
        {  
            Temperature = (float)0.7,  
            MaxOutputTokenCount = 800,  
            
            FrequencyPenalty = 0,  
            PresencePenalty = 0,  
        };  
    
        try  
        {  
            // 建立聊天完成要求
            ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);  
    
            // 列印回應
            if (completion.Content != null && completion.Content.Count > 0)
            {
                Debug.Log($"{completion.Content[0].Kind}: {completion.Content[0].Text}");
                return completion.Content[0].Text;
            } 
            else  
            {  
                Debug.Log("No response received.");  
            }  
        }  
        catch (Exception ex)  
        {  
            Debug.Log($"An error occurred: {ex.Message}");  
        } 
        return ""; 
    }
}
