namespace application.Services.Interfaces
{
    using model.Enums;

    public interface IQueryIntentClassifier
    {
        Task<QueryIntent> ClassifyAsync(string userMessage, CancellationToken ct = default);
       
    }
}
