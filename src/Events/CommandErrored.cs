using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Exceptions;
using Pooja.src.Attributes;
using Pooja.src.Exceptions.Economy;
using Pooja.src.Exceptions.General;

namespace Pooja.src.Events;

[CommandsNextEvent]
public static class CommandErrored
{
    public static async Task MainAsync(CommandsNextExtension _, CommandErrorEventArgs args)
    {
        var context = args.Context;
        var exception = args.Exception;

        switch (exception)
        {
            case ChecksFailedException checks:
                await HandleFailedChecksAsync(context, checks);
                break;

            case CommandNotFoundException:
                await context.RespondAsync("`that command does not exist`");
                break;

            case ArgumentException or ArgumentNullException:
                await context.RespondAsync(exception.Message);
                break;

            case NotFoundException:
                break;

            case PlayerNotFoundException or PlayerAlreadyExistsException or PoojaAdminAlreadyExistsException or PoojaAdminNotFoundException or PoojaGeneralException or EconomyException:
                await context.RespondAsync(exception.Message);
                break;

            default:
                await context.RespondAsync($"`an unexpected error occurred while executing the command: {exception.Message}`");
                Console.WriteLine($"[ERROR] {exception.Message}\n{exception.StackTrace}");
                break;
        }
    }

    private static async Task HandleFailedChecksAsync(CommandContext context, ChecksFailedException exception)
    {
        foreach (var check in exception.FailedChecks)
        {
            var message = check switch
            {
                IsOwnerAttribute => "`this command is restricted to only the owner, which you are not`",
                CooldownAttribute cooldown => $"`you can use this command again in {cooldown.GetRemainingCooldown(context).TotalSeconds:F1} seconds`",
                IsAdminAttribute => "`this command requires an administrator, which you are not`",
                RequireUserPermissionsAttribute requireUserPermissions => $"`you are missing permissions {string.Join(", ", requireUserPermissions.Permissions)}`",
                RequireBotPermissionsAttribute requireBotPermissions => $"`i am missing permissions {string.Join(", ", requireBotPermissions.Permissions)}`",
                IsPlayerAttribute _ => $"`you are not a registered player. register using the command 'create'`",
                _ => $"`requirement check failed. requirement is: {check}`"
            };

            await context.RespondAsync(message);
            return;
        }
    }
}