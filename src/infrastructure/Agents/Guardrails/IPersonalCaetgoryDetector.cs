namespace infrastructure.Agents.Guardrails
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IPersonalCaetgoryDetector
    {
        Task<CustomCategoryAnalysisResponse> DetectPII(string text, string categoryName);
    }
}
