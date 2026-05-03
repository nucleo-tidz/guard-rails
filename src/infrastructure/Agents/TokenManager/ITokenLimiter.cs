namespace infrastructure.Agents.TokenManager
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface ITokenLimiter
    {
        Task<bool> IsAllowed();
        Task ConsumeAsync(long tokens);
    }
}
