namespace infrastructure.RateLimiters
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IFixedWindowLimiter
    {
        Task<bool> IsRequestAllowedAsync(string key);
    }
}
