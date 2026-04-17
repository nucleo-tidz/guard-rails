namespace infrastructure.Agents.Guardrails
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal interface IGroundnessDetector
    {
        Task<GroundednessDetectionResponse> DetectGroundness(string userQuery, string reponse, List<string> knowledgeBase);
    }
}
