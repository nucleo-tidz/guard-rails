namespace infrastructure.Agents.AzureAgents
{
    using System.Threading.Tasks;

    public interface INucleotidzAgent
    {
        Task<string> Execute(string input, string userName, string threadId);
    }
}
