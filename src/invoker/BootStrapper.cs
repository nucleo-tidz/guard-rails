namespace invoker
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using infrastructure.Agents.AzureAgents;


    internal class BootStrapper(INucleotidzAgent nucleotidzAgent) : IBootStrapper
    {
        public async Task Start(CancellationToken cancellationToken)
        {
            await nucleotidzAgent.Execute(Console.ReadLine(),string.Empty,string.Empty);
        }
    }
}
