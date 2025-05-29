using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Pooja.src.Services;

namespace Pooja.src.Attributes;

public class IsPlayerAttribute : CheckBaseAttribute
{
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        var economyService = ctx.Services.GetRequiredService<EconomyService>();
        var economyUser = economyService.GetPoojaEconomyUserAsync(ctx.User.Id);

        return Task.FromResult(economyUser is not null);
    }
}