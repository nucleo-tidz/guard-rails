namespace application.Services
{
    using application.Services.Interfaces;
    using System.Threading.Tasks;

    public class NucleotidzAgentService(INucleotidzAgent nucleotidzAgent) : INucleotidzAgentService
    {
        public async Task<string> Start(string message)
        {
           return await nucleotidzAgent.Start(message);
        }
    }
}
