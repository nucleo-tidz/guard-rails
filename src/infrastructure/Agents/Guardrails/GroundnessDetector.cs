namespace infrastructure.Agents.Guardrails
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;

    using Microsoft.Extensions.Configuration;

    public record GroundednessDetectionResponse(bool UngroundedDetected, double UngroundedPercentage, List<UngroundedDetail> UngroundedDetails);
    public record UngroundedDetail(string Text);
    internal class GroundnessDetector(HttpClient httpClient) : IGroundnessDetector
    {
        public async Task<GroundednessDetectionResponse> DetectGroundness( string userQuery, string reponse, List<string> knowledgeBase)
        {
            var requestBody = new
            {
                domain = "Generic",
                task = "QnA",
                text = reponse,
                groundingSources = knowledgeBase.ToArray(),
                qna = new
                {
                    query = userQuery
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/contentsafety/text:detectGroundedness?api-version=2024-09-15-preview", content);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<GroundednessDetectionResponse>(result, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return data;
            }
            throw new Exception($"Error analyzing groundness: {response.ReasonPhrase} - {await response.Content.ReadAsStringAsync()}");
        }
    }
}
