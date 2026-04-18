namespace infrastructure.Agents.Guardrails
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;

    public record UserPromptAnalysis(bool AttackDetected);
    public record DocumentAnalysis(bool AttackDetected);
    public record JailbreakDetectionResponse(UserPromptAnalysis UserPromptAnalysis, List<DocumentAnalysis> DocumentsAnalysis);
    internal class JailBreakDetector(HttpClient httpClient):IJailBreakDetector
    {
        public async Task<JailbreakDetectionResponse> DetectJailBreak(string prompt, List<string> knowledgeBase)
        {
            var requestBody = new
            {
                userPrompt = prompt,
                documents = knowledgeBase.ToArray()
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/contentsafety/text:shieldPrompt?api-version=2024-09-15-preview", content);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JailbreakDetectionResponse>(result, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return data;
            }
            throw new Exception($"Error analyzing jail break: {response.ReasonPhrase} - {await response.Content.ReadAsStringAsync()}");
        }
    }
}
