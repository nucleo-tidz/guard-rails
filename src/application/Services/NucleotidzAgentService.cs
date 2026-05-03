namespace application.Services
{
    using application.Services.Interfaces;

    using model;

    using System.Threading.Tasks;

    public class NucleotidzAgentService(INucleotidzAgent nucleotidzAgent, IOnBoardRepository onBoardRepository) : INucleotidzAgentService
    {
        public async Task OnBoard(TokenLimitModel tokenLimitModel)
        {
            await onBoardRepository.OnBoardAsync(tokenLimitModel);
        }
        public async Task<string> Start(string message)
        {
            return await nucleotidzAgent.Start(message);
        }
    }
}
