using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Pooja.src.Services;

namespace Pooja.src.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class IsAdminAttribute : CheckBaseAttribute
{
    public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        var poojaService = ctx.Services.GetRequiredService<GeneralPoojaService>();
        var admins = await poojaService.GetPoojaAdminsAsync(); 

        return admins.Any(x => x.ID == ctx.User.Id && x.Position == PoojaHierarchy.Admin);
    }
}