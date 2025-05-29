using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Pooja.src.Attributes;
using Pooja.src.Extensions;
using Pooja.src.Services;
using Pooja.src.Utils;

namespace Pooja.src.Modules;

public class OwnerModule : BaseCommandModule
{
    public required PoojaConfig Config { get; set; }
    public required GeneralPoojaService GeneralPoojaService { get; set; }

    [Aliases("aadmin", "appendadmin")]
    [Command("addadmin")]
    [IsOwner]
    public async Task AddAdminAsync(CommandContext context, DiscordMember member, string position, [RemainingText] string name)
    {
        var hierarchy = PoojaGeneralUtils.ConvertPoojaHierarchy(position);

        await GeneralPoojaService.CreatePoojaAdminAsync(member.Id, name, hierarchy);
        await context.RespondAsync($"`{name} - {member.Id}` has been added to the admin list with `{position.ToLower()}` permissions");
    }

    [Aliases("ladmins", "gadmins", "getadmins")]
    [Command("listadmins")]
    public async Task GetAdminsAsync(CommandContext context)
    {
        var admins = await GeneralPoojaService.GetPoojaAdminsAsync();
        var interactivity = context.Client.GetInteractivity();

        var pages = await interactivity.ToDiscordPages(
            admins,
            titleSelector: x => x.Name,
            colorSelector: x => new DiscordColor(0x503dfc),
            footerSelector: x => ("a list of admins within this server", context.Client.CurrentUser.AvatarUrl),
            thumbnailSelector: async x =>
            {
                var user = await context.Client.GetUserAsync(x.ID);

                return user.AvatarUrl;
            },

            fieldSelectors: x => ("position", PoojaGeneralUtils.ConvertPoojaHierarchy(x.Position))
        );

        var message = await interactivity.AddNavigationReactionsAsync(context, pages);

        await interactivity.DoNavigationEmojisAsync(context, message, TimeSpan.FromSeconds(30), pages);
    }
}