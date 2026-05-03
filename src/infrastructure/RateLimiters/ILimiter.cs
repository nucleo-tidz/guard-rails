namespace infrastructure.RateLimiters
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface ILimiter
    {
        Task<bool> IsRequestAllowedAsync(string key);
    }
}
