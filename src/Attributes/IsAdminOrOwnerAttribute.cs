using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Pooja.src.Exceptions.General;
using Pooja.src.Services;

namespace Pooja.src.Attributes;

public class IsAdminOrOwnerAttribute : CheckBaseAttribute
{
    public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        bool isAdminOrOwner = false;

        try
        {
            var poojaService = ctx.Services.GetRequiredService<GeneralPoojaService>();
            var admin = await poojaService.GetPoojaAdminAsync(ctx.User.Id);

            isAdminOrOwner = true;
        }
        catch (PoojaAdminNotFoundException)
        {
            // do nothing
        }

        var configService = ctx.Services.GetRequiredService<PoojaConfig>();
        var owners = configService.OwnerIDs;

        isAdminOrOwner = owners
            .Any(x => ctx.User.Id == x);

        return isAdminOrOwner;
    }
}