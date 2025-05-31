using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Pooja.src.Attributes;
using Pooja.src.Extensions;
using Pooja.src.Services;
using Pooja.src.Utils;

namespace Pooja.src.Modules;

public class EconomyModule : BaseCommandModule
{
    public required EconomyService EconomyService { get; set; }

    [Command("balance")]
    [Aliases("bal")]
    [Description("Allows you to view your balance or someone else's")]
    [IsPlayer]
    public async Task Balance(CommandContext context, DiscordUser? user = null)
    {
        user ??= context.User;

        var found = await EconomyService.GetPoojaEconomyUserAsync(user.Id);
        var embed = new DiscordEmbedBuilder();

        embed.WithThumbnail(user.AvatarUrl);

        embed.WithTitle($"{user.Username}'s balance");
        embed.WithDescription($"Bank Balance: `{EconomyUtils.FormatCurrency(found.Bank)}`\nCash Balance: `{EconomyUtils.FormatCurrency(found.Cash)}`\n Total Balance: `{EconomyUtils.FormatCurrency(found.Bank + found.Cash)}`");

        embed.WithColor(0x503dfc);

        embed.WithFooter("balance signifies how much a person has", context.Client.CurrentUser.AvatarUrl);

        await context.RespondAsync(embed);
    }

    [Command("create")]
    [Aliases("join", "joineconomy")]
    [Description("Go outside if you use this command")]
    public async Task CreateProfileAsync(CommandContext context)
    {
        await EconomyService.CreatePoojaEconomyUser(context.User.Id);

        await context.RespondAsync($"welcome to pooja's economy, `{context.User.Username}`. don't be stupid");
    }

    [Command("rob")]
    [Aliases("steal", "burglarize", "blackerize", "blackerise", "burglarise", "robplayer")]
    [Description("Basically pulls a George Floyd on players with cash in their wallets")]
    [IsPlayer]
    public async Task RobPlayerAsync(CommandContext context, DiscordMember victim)
    {
        var player = await EconomyService.GetPoojaEconomyUserAsync(victim.Id);

        if (player.Cash < 500)
        {
            await context.RespondAsync("you can't steal from someone with no money in their pockets...can't have shit in detroit");
            return;
        }

        if (victim == context.Member)
        {
            // this is actually 1 in 100,001 chances, making it harder for you to duplicate your own wallet's currency
            if (Random.Shared.NextInt64(0, 100_000) == 1)
            {
                await EconomyService.UpdatePoojaUserAsync(victim.Id, player.Cash * 2);

                await context.RespondAsync($"i guess i am really dumb. you've duplicated your wallet amount");
                return;
            }
            else
            {
                await context.RespondAsync("nice try. you really thought i'd be flawed, cheers to you!");
                return;
            }
        }

        var toBeStealed = Random.Shared.NextLongWithinRange(500, player.Cash);

        // gta 5 canonical lore, lamar and franklin only pulled off a heist (technically, more of a robbery) of up to 2 dye pack stacks split.
        // i've only added an extra 2,500 because of pooja lore
        var builder = new DiscordEmbedBuilder()
            .WithThumbnail(victim.AvatarUrl)
            .WithTitle(victim.Username)
            .WithColor(0x503dfc);

        if (toBeStealed <= 4_500)
        {
            builder.WithFooter("some franklin and lamar ass robbery", context.Client.CurrentUser.AvatarUrl);
        }
        else if (toBeStealed >= 10_000)
        {
            builder.WithFooter("some michael ass robbery, nice job", context.Client.CurrentUser.AvatarUrl);
        }
        else if (toBeStealed == player.Cash)
        {
            builder.WithFooter("some aiden pearce + damien brenks shit. you've stolen everything, excellent work", context.Client.CurrentUser.AvatarUrl);
        }
        else
        {
            builder.WithFooter("i really don't know what to call this one, i guess nice work?", context.Client.CurrentUser.AvatarUrl);
        }

        await EconomyService.ChangePoojaUserBalanceAsync(victim.Id, cash: -toBeStealed);
        await EconomyService.ChangePoojaUserBalanceAsync(context.User.Id, cash: +toBeStealed);

        var changed = await EconomyService.GetPoojaEconomyUserAsync(victim.Id);

        builder.WithDescription(
            $"{context.User.Username} has stolen `{toBeStealed:##,#}` from {victim.Mention}\n-------------\n" +
            $"{victim.Mention} now has `{changed.Cash:##,#}`"
        );

        await context.RespondAsync(builder);
    }

    [Command("leaderboard")]
    [Aliases("getleaderboard")]
    [Description("Shows a leaderboard containing players and their stats")]
    [IsPlayer]
    public async Task GetLeaderboardAsync(CommandContext context)
    {
        var players = await EconomyService.GetPoojaEconomyUsersAsync();

        if (players.Count == 0)
        {
            await context.RespondAsync("no have been players found...");
            return;
        }

        var embedBuilder = new DiscordEmbedBuilder()
            .WithTitle("leaderboard")
            .WithColor(0x503dfc);

        StringBuilder builder = new();

        var count = 1;

        foreach (var player in players.OrderByDescending(x => x.Bank + x.Cash))
        {
            var user = await context.Guild.GetMemberAsync(player.ID);

            builder.AppendLine($"`[{((long)count).Ordinal()}] - {user.DisplayName}`\n`Total balance: {EconomyUtils.FormatCurrency(player.Bank + player.Cash)}`\n");

            count++;
        }

        embedBuilder.WithDescription(builder.ToString());

        await context.RespondAsync(embedBuilder);
    }

    [Command("give")]
    [Aliases("giveplayer", "sendmoney")]
    [Description("Allows you to give money to a player")]
    [IsPlayer]
    public async Task GiveMoneyAsync(CommandContext context, DiscordMember member, long amount = 500)
    {
        if (amount <= 0)
        {
            await context.RespondAsync($"you can't send nothing to {member.Mention}");
            return;
        }

        var sender = await EconomyService.GetPoojaEconomyUserAsync(context.User.Id);
        var receiver = await EconomyService.GetPoojaEconomyUserAsync(member.Id);

        if (amount > sender.Cash)
        {
            await context.RespondAsync($"you can't send more than `{EconomyUtils.FormatCurrency(amount)}` to {member.Mention}");
            return;
        }

        var embedBuilder = new DiscordEmbedBuilder()
            .WithTitle("cash transaction")
    }
}