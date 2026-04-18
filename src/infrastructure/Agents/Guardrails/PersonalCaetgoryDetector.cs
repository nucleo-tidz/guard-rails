namespace infrastructure.Agents.Guardrails
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;

    using static System.Net.Mime.MediaTypeNames;

    public record CustomCategoryAnalysisResponse(CustomCategoryAnalysis customCategoryAnalysis);
    public record CustomCategoryAnalysis(bool detected);
    internal class PersonalCaetgoryDetector(HttpClient httpClient): IPersonalCaetgoryDetector
    {
        public async Task<CustomCategoryAnalysisResponse> DetectPII(string text, string categoryName)
        {
            var requestBody = new
            {
                Text = text,
                CategoryName = categoryName,
                Version = 1
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/contentsafety/text:analyzeCustomCategory?api-version=2024-02-15-preview", content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<CustomCategoryAnalysisResponse>(result, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return data;

            }
            throw new Exception($"Error analyzing custom category: {response.ReasonPhrase} - {await response.Content.ReadAsStringAsync()}");
        }
    }
}
