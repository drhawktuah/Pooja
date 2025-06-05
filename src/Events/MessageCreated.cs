using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Pooja.src.Attributes;
using Pooja.src.Services;

namespace Pooja.src.Events;


[DiscordClientEvent]
public static class MessageCreated
{
    public static async Task MainAsync(DiscordClient client, MessageCreateEventArgs args)
    {
        var author = args.Author;

        if (author.IsBot)
            return;

        var message = args.Message;

        var commandsNext = client.GetCommandsNext();

        var fuzzyService = commandsNext.Services.GetRequiredService<PoojaFuzzyMatchingService>();
        var config = commandsNext.Services.GetRequiredService<PoojaConfig>();

        var (messagePosition, matchedPrefix) = FindMatchingPrefixes(config, message);

        if (messagePosition == -1 || matchedPrefix == null)
            return;

        var commandString = message.Content[messagePosition..]
            .Trim();

        if (string.IsNullOrWhiteSpace(commandString))
            return;

        var parts = commandString.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var (commandName, rawArguments) = (
            parts.Length > 0 ? parts[0] : string.Empty,
            parts.Length > 1 ? parts[1] : string.Empty
        );

        var command = commandsNext.FindCommand(commandName, out var _);
        if (command == null)
        {
            var results = fuzzyService.GetResults(commandName);
            var embed = await fuzzyService.ToDiscordEmbed(commandName, results);

            await message.Channel.SendMessageAsync(embed);

            return;
        }

        await commandsNext.ExecuteCommandAsync(commandsNext.CreateContext(message, matchedPrefix, command, rawArguments));
    }

    private static (int, string?) FindMatchingPrefixes(PoojaConfig config, DiscordMessage message)
    {
        foreach (var prefix in config.Prefixes)
        {
            int index = message.GetStringPrefixLength(prefix);
            if (index != -1)
                return (index, prefix);
        }

        return (-1, null);
    }
}
