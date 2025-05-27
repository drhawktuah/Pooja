using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Pooja.src.Attributes;

/// <summary>
/// `IsOwnerAttribute`. Used to mark commands as owner only
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class IsOwnerAttribute : CheckBaseAttribute
{
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        var config = ctx.Services.GetRequiredService<PoojaConfig>();

        return Task.FromResult(config.OwnerIDs.Any(x => x == ctx.User.Id));
    }
}
