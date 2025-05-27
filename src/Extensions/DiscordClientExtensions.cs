using System.Reflection;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Pooja.src.Attributes;

namespace Pooja.src.Extensions;

public static partial class DiscordClientExtensions
{
    public static async Task RegisterEventsAsync(this DiscordClient client)
    {
        var commandsNext = client.GetCommandsNext();

        foreach (var discordEvent in GetBotEvents<DiscordClientEventAttribute>())
        {
            await RegisterEventAsync(client, discordEvent);
        }

        foreach (var commandsEvent in GetBotEvents<CommandsNextEventAttribute>())
        {
            await RegisterEventAsync(commandsNext, commandsEvent);
        }

        await Task.CompletedTask;
    }

    private static async Task RegisterEventAsync(object target, Type eventType)
    {
        var type = target.GetType();

        var eventInfo = type.GetEvent(eventType.Name);
        if (eventInfo is null)
        {
            Console.WriteLine($"{eventType.Name} is not a valid event on {nameof(target)}");
            return;
        }

        var method = eventType.GetMethod("MainAsync");
        if (method == null)
        {
            Console.WriteLine($"{eventType.Name} does not contain a 'MainAsync' method");
            return;
        }

        if (eventInfo.EventHandlerType == null)
        {
            Console.WriteLine($"{eventInfo.Name} EventHandlerType is null");
            return;
        }

        try
        {
            var delegateInstance = method.CreateDelegate(eventInfo.EventHandlerType);
            eventInfo.AddEventHandler(target, delegateInstance);

            Console.WriteLine($"{eventInfo.Name} initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error initializing {eventInfo.Name}: {ex}");
        }

        await Task.CompletedTask;
    }

    private static IEnumerable<Type> GetBotEvents<TAttribute>() where TAttribute : Attribute
    {
        return typeof(TAttribute).Assembly
            .GetTypes()
            .Where(static type => type.GetCustomAttribute<TAttribute>() is not null && type.GetMethod("MainAsync") is not null);
    }
}