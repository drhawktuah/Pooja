using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Pooja.src.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class IsAdminAttribute : CheckBaseAttribute
{
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        var config = ctx.Services.GetRequiredService<PoojaConfig>();

        return Task.FromResult(config.AdminIDs.Any(x => x == ctx.User.Id));
    }
}