using System.Reflection;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pooja.src.Extensions;
using Pooja.src.Services;

namespace Pooja.src;

public sealed class Pooja
{
    private readonly PoojaConfig config;
    private readonly DiscordClient client;
    private readonly CommandsNextExtension commandsNext;

    public DiscordClient Client => client;

    public Pooja()
    {
        config = PoojaConfig.Deserialize("src//JSON//config.json");
        client = new DiscordClient(BuildDiscordConfig());
        client.UseInteractivity(BuildInteractivityConfig());

        var services = ConfigureServices();

        commandsNext = client.UseCommandsNext(BuildCommandsNextConfig(config, services));
        commandsNext.RegisterCommands(Assembly.GetExecutingAssembly());
        commandsNext.SetHelpFormatter<PoojaHelpFormatter>();
    }

    public async Task StartAsync()
    {
        await client.RegisterEventsAsync();
        await client.InitializeAsync();
        await client.ConnectAsync();

        await Task.Delay(-1);
    }

    private DiscordConfiguration BuildDiscordConfig() => new()
    {
        Token = config.Token,
        TokenType = TokenType.Bot,
        Intents = DiscordIntents.All,
        AutoReconnect = true,
        MinimumLogLevel = LogLevel.Information
    };

    private ServiceProvider ConfigureServices()
    {
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var mongoDatabase = mongoClient.GetDatabase("Pooja");

        var services = new ServiceCollection()
            .AddSingleton(config)
            .AddSingleton(new Random())
            .AddSingleton(new RandomHouseService("src//Assets//House", "src//Assets//house.txt"))
            .AddSingleton(mongoClient)
            .AddSingleton(mongoDatabase)
            .AddSingleton<EconomyService>()
            .AddSingleton<GeneralPoojaService>()
            .AddSingleton(sp =>
                new PoojaFuzzyMatchingService(commandsNext))
            .BuildServiceProvider();

        return services;
    }

    private static CommandsNextConfiguration BuildCommandsNextConfig(PoojaConfig config, IServiceProvider services) => new()
    {
        CaseSensitive = false,
        UseDefaultCommandHandler = false,
        Services = services
    };

    private static InteractivityConfiguration BuildInteractivityConfig() => new()
    {
        Timeout = TimeSpan.FromSeconds(30),
        PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.DeleteEmojis
    };
}
