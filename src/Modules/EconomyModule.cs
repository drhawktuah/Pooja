using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Pooja.src.Attributes;
using Pooja.src.Services;
using ZstdSharp.Unsafe;

namespace Pooja.src.Modules;

public class EconomyModule : BaseCommandModule
{
    public required EconomyService EconomyService { get; set; }

    [Command("balance")]
    [Aliases("bal")]
    [Description("Allows you to view your balance or someone else's")]
    [IsPlayer]
    public async Task Balance(CommandContext context, DiscordUser? user = null) {
        user ??= context.User;

        var found = await EconomyService.GetPoojaEconomyUserAsync(user.Id);
        var embed = new DiscordEmbedBuilder();

        embed.WithTitle($"{user.Username}'s balance");
        embed.WithDescription($"Bank Balance: {found.Bank}\nCash Balance: {found.Cash}\nTotal Balance: {found.Bank + found.Cash}");

        embed.WithColor(0x503dfc);

        await context.RespondAsync(embed);
    }
}