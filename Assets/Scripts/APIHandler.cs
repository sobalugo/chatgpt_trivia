using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class APIHandler : MonoBehaviour
{
    [SerializeField] private string chatGPTAPIKey = "insert_API_key_from_OpenAI";
    private const string chatGPTAPIEndpoint = "https://api.openai.com/v1/chat/completions";
    private List<string> askedQuestions = new List<string>();

    public void GenerateQuestion(string category, Action<string> callback)
    {
        // Create a string with the summary of asked questions, escaping special characters
        string askedQuestionsSummary = string.Join(", ", askedQuestions.Select(q => EscapeSpecialCharacters(q)));

        // Include the asked questions summary in the prompt
        string prompt = $"{category}. Don't give questions that are similar to the ones that were already asked. Respond with the question only and nothing else.";
        if (!string.IsNullOrEmpty(askedQuestionsSummary))
        {
            prompt = $"Given the following asked questions: {askedQuestionsSummary}. Now, {prompt}";
        }

        StartCoroutine(SendAPIRequest(prompt, response =>
        {
            string question = ParseResponse(response);

            // Append the new question to the list of asked questions
            askedQuestions.Add(question);

            callback(question);
        }));
    }

    public void EvaluateAnswer(string question, string answer, Action<bool> callback)
    {
        string escapedQuestion = EscapeSpecialCharacters(question);
        string escapedAnswer = EscapeSpecialCharacters(answer);
        string prompt = $"Is '{escapedAnswer}' the correct answer for the question '{escapedQuestion}'? You may have some leeway with spelling mistakes. Answer only yes or no.";
        StartCoroutine(SendAPIRequest(prompt, response =>
        {
            string evaluation = ParseResponse(response).ToLower();
            bool correct = evaluation.Contains("yes") || evaluation.Contains("correct");
            callback(correct);
        }));
    }

    private IEnumerator SendAPIRequest(string prompt, Action<string> callback)
    {
        using (UnityWebRequest request = new UnityWebRequest(chatGPTAPIEndpoint, "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {chatGPTAPIKey}");

            string requestBody = $"{{\"model\": \"gpt-4\", \"messages\": [{{\"role\": \"user\", \"content\": \"{prompt}\"}}]}}";
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(requestBody));
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                string response = request.downloadHandler.text;
                callback(response);
            }
        } // The UnityWebRequest object will be disposed of here
    }

    private string ParseResponse(string response)
    {
        JObject jsonResponse = JObject.Parse(response);
        string content = jsonResponse["choices"][0]["message"]["content"].ToString();
        return content;
    }

    private string EscapeSpecialCharacters(string input)
    {
        return input.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t");
    }

    public void ClearAskedQuestions()
    {
        askedQuestions.Clear();
    }
}
