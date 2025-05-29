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
    private static readonly PoojaConfig poojaConfig = PoojaConfig.Deserialize("src//JSON//config.json");

    private static readonly Random random = new();
    private static readonly RandomHouseService houseService = new("src//Assets//House", "src//Assets//house.txt");

    private static readonly MongoClient mongoClient = new("mongodb://localhost:27017");
    private static readonly IMongoDatabase mongoDatabase = mongoClient.GetDatabase("Pooja");
    private static readonly EconomyService economyService = new(mongoDatabase);
    private static readonly GeneralPoojaService generalPoojaService = new(mongoDatabase);

    public readonly DiscordClient Client;
    private readonly CommandsNextExtension CommandsNext;

    private static readonly ServiceProvider services = new ServiceCollection()
        .AddSingleton(poojaConfig)
        .AddSingleton(random)
        .AddSingleton(houseService)
        .AddSingleton(mongoClient)
        .AddSingleton(mongoDatabase)
        .AddSingleton(economyService)
        .AddSingleton(generalPoojaService)
        .BuildServiceProvider();

    internal Pooja()
    {
        Client = new(GetDiscordConfiguration(poojaConfig));
        Client.UseInteractivity(GetInteractivityConfiguration());

        CommandsNext = Client.UseCommandsNext(GetCommandsNextConfiguration(poojaConfig));
        CommandsNext.RegisterCommands(Assembly.GetExecutingAssembly());
        CommandsNext.SetHelpFormatter<PoojaHelpFormatter>();
    }

    public async Task Start()
    {
        await Client.RegisterEventsAsync();

        await Client.InitializeAsync();
        await Client.ConnectAsync();

        await Task.Delay(-1);
    }

    public static DiscordConfiguration GetDiscordConfiguration(PoojaConfig config) => new()
    {
        Token = config.Token,
        TokenType = TokenType.Bot,
        Intents = DiscordIntents.All,
        MinimumLogLevel = LogLevel.Information,
        AutoReconnect = true
    };

    public static CommandsNextConfiguration GetCommandsNextConfiguration(PoojaConfig config) => new()
    {
        StringPrefixes = config.Prefixes,
        CaseSensitive = false,
        Services = services
    };

    public static InteractivityConfiguration GetInteractivityConfiguration() => new()
    {
        PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.DeleteEmojis,
        Timeout = TimeSpan.FromSeconds(30)
    };
}
