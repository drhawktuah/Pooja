using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Pooja.src.Services;

namespace Pooja.src.Modules;

public class FunModule : BaseCommandModule
{
    public required RandomHouseService HouseService { get; set; }

    [Aliases("randomhousequote", "rhq", "randomhouse", "roadhouse")]
    [Command("house")]
    [RequireGuild]
    [Cooldown(2, 8, CooldownBucketType.Global)]
    public async Task SendRandomHouseQuote(CommandContext context) {
        var randomQuote = HouseService.GetRandomQuote();

        var embed = new DiscordEmbedBuilder()
            .WithTitle(randomQuote)
            .WithThumbnail(context.Client.CurrentUser.AvatarUrl)
            .WithDescription("-*Gregory House, 2025*")
            .WithColor(0x503dfc);

        var message = new DiscordMessageBuilder()
            .WithEmbed(embed);

        await context.Channel.SendMessageAsync(message);
    }
}