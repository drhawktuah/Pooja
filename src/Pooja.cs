using System.Reflection;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pooja.src.Extensions;

namespace Pooja.src;

public sealed class Pooja
{
    public static readonly PoojaConfig Config = PoojaConfig.Deserialize("src//config.jsonc");
    public static readonly Random Random = new();
    public readonly DiscordClient Client;

    private readonly CommandsNextExtension CommandsNext;
    private static readonly ServiceProvider services = new ServiceCollection()
        .AddSingleton(Config)
        .AddSingleton(Random)
        .BuildServiceProvider();

    internal Pooja()
    {
        Client = new(GetDiscordConfiguration(Config));
        Client.UseInteractivity(GetInteractivityConfiguration());

        CommandsNext = Client.UseCommandsNext(GetCommandsNextConfiguration(Config));
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
