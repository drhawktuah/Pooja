using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Pooja.src.Modules;

public class GeneralModule : BaseCommandModule
{
    [Command("serverinfo")]
    [RequireGuild]
    [Aliases("svi", "guildinfo")]
    [Description("Displays information about a server")]
    public async Task ServerInfoAsync(CommandContext context)
    {
        var guild = context.Guild;
        var embed = new DiscordEmbedBuilder();

        embed.WithTitle(guild.Name);
        embed.WithThumbnail(guild.IconUrl);

        var builder = new StringBuilder()
            .AppendLine($"created at: `{guild.CreationTimestamp:dd-MM-yyyy HH:mm}`")
            .AppendLine($"bot count: `{guild.Members.Count(x => x.Value.IsBot)}`")
            .AppendLine($"human count: `{guild.Members.Count(x => !x.Value.IsBot)}`")
            .AppendLine($"total member count: `{guild.MemberCount}`")
            .AppendLine($"server boost level: `{guild.PremiumTier}`")
            .AppendLine($"server boost count: `{(guild.PremiumSubscriptionCount is null or 0 ? "no boosters..." : guild.PremiumSubscriptionCount)}`")
            .AppendLine($"category channels count: `{guild.Channels.Count(x => x.Value.Type == DSharpPlus.ChannelType.Category)}`")
            .AppendLine($"text channels count: `{guild.Channels.Count(x => x.Value.Type == DSharpPlus.ChannelType.Text)}`")
            .AppendLine($"voice channels count: `{guild.Channels.Count(x => x.Value.Type == DSharpPlus.ChannelType.Voice)}`")
            .AppendLine($"total talking channels count: `{guild.Channels.Count(x => x.Value.Type != DSharpPlus.ChannelType.Category)}`");

        embed.WithDescription(builder.ToString());

        await context.RespondAsync(embed);
    }

    [Command("whois")]
    [RequireGuild]
    [Aliases("userinfo")]
    [Description("Displays information about a server member")]
    public async Task MemberInfoAsync(CommandContext context, DiscordMember? member = null)
    {
        member ??= context.Member;

        var embed = new DiscordEmbedBuilder()
            .WithTitle(member?.Username);

        embed.WithThumbnail(member?.GetGuildAvatarUrl(DSharpPlus.ImageFormat.Png));

        var builder = new StringBuilder()
            .AppendLine($"created account on: `{member?.CreationTimestamp:dd-MM-yyyy HH:mm}`")
            .AppendLine($"joined server on: `{member?.JoinedAt:dd-MM-yyyy HH:mm}`")
            .AppendLine($"id: `{member?.Id}`")
            .AppendLine($"is a bot: `{member?.IsBot}`")
            .AppendLine($"roles: `{string.Join(", ", member?.Roles.Where(x => x != context.Guild.EveryoneRole).Select(x => x.Name))}`")
            .AppendLine($"highest role: `{member?.Roles.First().Name}`")
            .AppendLine($"is owner: `{member?.IsOwner}`")
            .AppendLine($"is a bot: `{member?.IsBot}`");

        embed.WithDescription(builder.ToString());

        if (context.Member == member)
        {
            embed.WithFooter($"you already know who requested this", context.Client.CurrentUser.AvatarUrl);
        }
        else
        {
            embed.WithFooter($"requested by {context.User.Username}", context.User.AvatarUrl);
        }

        await context.RespondAsync(embed);
    }
}