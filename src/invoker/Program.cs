
using infrastructure;

using invoker;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();

    })
    .ConfigureServices((config, services) =>
    {
        services.AddSemanticKernel(config.Configuration);
        services.AddAzureAIAgent(config.Configuration);

        // services.AddAgents();
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddTransient<IBootStrapper, BootStrapper>();

#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }).Build();

IBootStrapper bootStrapper = host.Services.GetRequiredService<IBootStrapper>();
bootStrapper.Start(CancellationToken.None).Wait();