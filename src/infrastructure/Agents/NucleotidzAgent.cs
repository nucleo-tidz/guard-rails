namespace infrastructure.Agents
{
    using application.Services.Interfaces;
    using infrastructure.Agents.Factory;

    using model;

    internal class NucleotidzAgent(IAgentFactory agentFactory,ISharedContext sharedContext) : INucleotidzAgent
    {
        public async Task<string> Start(string message)
        {
            var agent = agentFactory.Create();
            var session = await agent.CreateSessionAsync();
            session.StateBag.SetValue("conversationId", sharedContext.ThreadId);
            session.StateBag.SetValue("UserId", sharedContext.User);
            var response = await agent.RunAsync(message, session);
            return response.Text;
        }
    }
}
