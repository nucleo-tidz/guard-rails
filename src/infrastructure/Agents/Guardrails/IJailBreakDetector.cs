namespace infrastructure.Agents.Guardrails
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IJailBreakDetector
    {
        Task<JailbreakDetectionResponse> DetectJailBreak(string prompt, List<string> knowledgeBase);
    }
}
