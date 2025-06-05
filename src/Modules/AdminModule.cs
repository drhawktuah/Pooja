using System.Text;
using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Pooja.src.Attributes;
using Pooja.src.Services;
using ZstdSharp.Unsafe;

namespace Pooja.src.Modules;

public class AdminModule : BaseCommandModule
{
    public required EconomyService EconomyService { get; set; }

    [Command("ban")]
    [RequireGuild]
    [Aliases("banuser", "banmember")]
    [Description("Bans a user either outside of the server or within the server")]
    [RequireUserPermissions(Permissions.BanMembers)]
    [RequireBotPermissions(Permissions.BanMembers)]
    public async Task BanMemberAsync(CommandContext context, DiscordMember member, int deletedDays = 0, string reason = "none provided")
    {
        if (member == context.Member)
        {
            await context.RespondAsync($"you cannot ban yourself");
            return;
        }

        if (member.Hierarchy > context.Member?.Hierarchy)
        {
            await context.RespondAsync($"you cannot ban `{member.Username}` due to their position being higher than yours");
            return;
        }

        if (member.Hierarchy > context.Guild.CurrentMember.Hierarchy)
        {
            await context.RespondAsync($"i cannot ban `{member.Username}` due to their position being higher than mine");
            return;
        }

        var memberRole = member.Roles.FirstOrDefault();

        if (memberRole is not null && (memberRole.Permissions.HasPermission(Permissions.BanMembers) | memberRole.Permissions.HasPermission(Permissions.KickMembers)))
        {
            await context.RespondAsync($"`{member.Username} has moderator permissions, are you sure you want to ban them?`");

            var interactivity = context.Client.GetInteractivity();
            var waited = await interactivity.WaitForMessageAsync(x => x.Channel == context.Channel && x.Author == context.User, TimeSpan.FromSeconds(15));

            if (waited.TimedOut)
            {
                await context.RespondAsync($"`timed out whilst authenticating request to ban {member.Username}`");
                return;
            }

            var result = waited.Result.Content.Trim().ToLowerInvariant();

            if (result is not ("yes" or "y"))
            {
                await context.RespondAsync($"`request to ban {member.Username} has been cancelled`");
                return;
            }
        }

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"Banned {member.Username}")
            .WithThumbnail(member.AvatarUrl);

        await member.BanAsync(deletedDays, reason);

        await context.Channel.SendMessageAsync(embed);
    }

    [Aliases("kickuser", "kickmember")]
    [Command("kick")]
    [Description("Kicks a user within the server")]
    [RequireGuild]
    [RequireBotPermissions(Permissions.KickMembers)]
    [RequireUserPermissions(Permissions.KickMembers)]
    public async Task KickMemberAsync(CommandContext context, DiscordMember member, string reason = "none provided")
    {
        if (member == context.Member)
        {
            await context.RespondAsync($"you cannot kick yourself");
            return;
        }

        if (member.Hierarchy > context.Member?.Hierarchy)
        {
            await context.RespondAsync($"you cannot kick `{member.Username}` due to their position being higher than yours");
            return;
        }

        if (member.Hierarchy > context.Guild.CurrentMember.Hierarchy)
        {
            await context.RespondAsync($"i cannot kick `{member.Username}` due to their position being higher than mine");
            return;
        }

        var memberRole = member.Roles.FirstOrDefault();

        if (memberRole is not null && (memberRole.Permissions.HasPermission(Permissions.BanMembers) | memberRole.Permissions.HasPermission(Permissions.KickMembers)))
        {
            await context.RespondAsync($"`{member.Username} has moderator permissions, are you sure you want to kick them?`");

            var interactivity = context.Client.GetInteractivity();
            var waited = await interactivity.WaitForMessageAsync(x => x.Channel == context.Channel && x.Author == context.User, TimeSpan.FromSeconds(15));

            if (waited.TimedOut)
            {
                await context.RespondAsync($"`timed out whilst authenticating request to kick {member.Username}`");
                return;
            }

            var result = waited.Result.Content.Trim().ToLowerInvariant();

            if (result is not ("yes" or "y"))
            {
                await context.RespondAsync($"`request to kick {member.Username} has been cancelled`");
                return;
            }
        }

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"Kicked {member.Username}")
            .WithThumbnail(member.AvatarUrl);

        await member.RemoveAsync(reason);

        await context.Channel.SendMessageAsync(embed);
    }

    [Aliases("purgemessages", "deletemessages")]
    [Command("purge")]
    [Description("Purges messages within a channel")]
    [RequireGuild]
    [RequireBotPermissions(Permissions.ManageMessages)]
    [RequireUserPermissions(Permissions.ManageChannels)]
    public async Task PurgeMessagesAsync(CommandContext context, int amount = 10, [RemainingText] string? filter = null)
    {

        if (!string.IsNullOrWhiteSpace(filter))
        {
            filter = Regex.Escape(filter.ToLowerInvariant());
        }

        var messages = await context.Channel.GetMessagesAsync(amount);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            messages = messages
                .Where(m => !m.Author.IsBot && Regex.IsMatch(m.Content.ToLowerInvariant(), filter))
                .ToList();
        }
        else
        {
            messages = messages
                .Where(m => !m.Author.IsBot)
                .ToList();
        }

        await context.Channel.DeleteMessagesAsync(messages);

        if (filter is not null)
        {
            await context.Channel.SendMessageAsync($"purged `{amount}` messages with {filter}");
        }
        else
        {
            await context.Channel.SendMessageAsync($"purged `{amount}` messages");
        }
    }

    [Command("unban")]
    [RequireGuild]
    [Aliases("unbanuser", "unbanmember")]
    [Description("Unbans a user")]
    [RequireUserPermissions(Permissions.BanMembers)]
    [RequireBotPermissions(Permissions.BanMembers)]
    public async Task UnbanMemberAsync(CommandContext context, DiscordUser user, string reason = "none provided")
    {
        if (user == context.User)
        {
            await context.RespondAsync($"you cannot unban yourself");
            return;
        }

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"Unbanned {user.Username}")
            .WithThumbnail(user.AvatarUrl);

        await context.Guild.UnbanMemberAsync(user, reason);
        await context.Channel.SendMessageAsync(embed);
    }

    [Command("lock")]
    [RequireGuild]
    [Aliases("lockchannel", "lockdown")]
    [Description("Locks a channel")]
    [RequireUserPermissions(Permissions.ManageChannels)]
    [RequireBotPermissions(Permissions.ManageChannels)]
    public async Task LockChannelAsync(CommandContext context)
    {
        var channel = context.Channel;

        await channel.AddOverwriteAsync(context.Guild.EveryoneRole, Permissions.None, Permissions.SendMessages, "lockdown");
        await channel.SendMessageAsync($"locked channel `{channel.Name}`");
    }

    [Command("unlock")]
    [RequireGuild]
    [Aliases("unlockchannel", "removelockdown")]
    [Description("Unlocks a channel")]
    [RequireUserPermissions(Permissions.ManageChannels)]
    [RequireBotPermissions(Permissions.ManageChannels)]
    public async Task UnlockChannelAsync(CommandContext context)
    {
        var channel = context.Channel;

        await channel.AddOverwriteAsync(context.Guild.EveryoneRole, Permissions.SendMessages, Permissions.None, "lockdown");
        await channel.SendMessageAsync($"unlocked channel `{channel.Name}`");
    }

    [Command("spamallchannels")]
    [RequireGuild]
    [Aliases("sac")]
    [Hidden]
    [Description("you know what this is big dawg, lamar davis on this description")]
    [RequireBotPermissions(Permissions.Administrator)]
    public async Task SendMessageToAllChannels(CommandContext context, [RemainingText] string message)
    {
        var guild = context.Guild;

        await Task.WhenAll(guild.Channels.Values.Where(x => x.Type == ChannelType.Text && !x.IsPrivate).Select(x => x.SendMessageAsync(message)));
    }

    [Command("addguild")]
    [Aliases("addpoojaguild", "ag", "apg")]
    [RequireGuild]
    [Hidden]
    [IsAdminOrOwner]
    [Description("adds a guild to the economy database")]
    public async Task CreatePoojaGuildAsync(CommandContext context, ulong channelID)
    {
        await EconomyService.AddPoojaGuildAsync(context.Guild.Id, channelID);

        await context.RespondAsync($"successfully added {channelID} to the database");
    }

    //add mute, unmute, warn, warnings and clearwarnings commands
}