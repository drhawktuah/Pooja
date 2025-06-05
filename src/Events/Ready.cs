using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Pooja.src.Attributes;

namespace Pooja.src.Events;

[DiscordClientEvent]
public static class Ready
{
    private readonly static DiscordActivity activity = new("ajoop", ActivityType.Streaming);

    public static async Task MainAsync(DiscordClient client, ReadyEventArgs _)
    {
        await client.UpdateStatusAsync(activity);

        Console.WriteLine($"Is this event name an AC/DC reference, {client.CurrentUser.Username} - {client.CurrentUser.Id}?");

        await Task.CompletedTask;
    }
}