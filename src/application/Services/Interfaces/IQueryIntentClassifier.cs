namespace application.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public enum QueryIntent
    {
        BookingQuery,      
        CompanyPolicy,     
        Mixed,           
        General            
    }

    public interface IQueryIntentClassifier
    {
        Task ClassifyAsync(string userMessage, CancellationToken ct = default);
        QueryIntent queryIntent { get; }
    }
}
