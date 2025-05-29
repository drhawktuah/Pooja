using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;

namespace Pooja.src.Extensions;


public static partial class DiscordInteractivityExtensions
{
    public static readonly Dictionary<string, DiscordEmoji> NavigationEmojis = new() {
    {
        "Start", DiscordEmoji.FromUnicode("⏪")
    },
    {
        "Backwards", DiscordEmoji.FromUnicode("⬅️")
    },
    {
        "Stop", DiscordEmoji.FromUnicode("⏹")
    },
    {
        "Forwards", DiscordEmoji.FromUnicode("➡️")
    },
    {
        "End", DiscordEmoji.FromUnicode("⏩")
    }
};

    public static async Task<DiscordMessage> AddNavigationReactionsAsync(this InteractivityExtension interactivity, CommandContext context, List<DiscordEmbed> embeds)
    {
        var embedMessage = await context.Channel.SendMessageAsync(embed: embeds[0]);

        foreach (var emoji in NavigationEmojis.Values)
        {
            await embedMessage.CreateReactionAsync(emoji);
        }

        return embedMessage;
    }

    public static async Task DoNavigationEmojisAsync(
        this InteractivityExtension interactivity,

        CommandContext context,
        DiscordMessage message,

        TimeSpan timeout,

        IReadOnlyList<DiscordEmbed> embeds)
    {
        int currentPage = 0;

        while (true)
        {
            var awaitedReaction = await interactivity.WaitForReactionAsync(x => ReactionPredicate(context, x), timeout);
            var result = awaitedReaction.Result;

            if (awaitedReaction.TimedOut)
            {
                await message.DeleteAsync();
                await context.RespondAsync("Navigation stopped");

                break;
            }

            if (result.Emoji == NavigationEmojis["Forwards"] && currentPage < embeds.Count - 1)
            {
                currentPage++;

                await message.ModifyAsync(embeds[currentPage]);
            }
            else if (result.Emoji == NavigationEmojis["End"] && currentPage < embeds.Count - 1)
            {
                currentPage = embeds.Count - 1;

                await message.ModifyAsync(embeds[currentPage]);
            }
            else if (result.Emoji == NavigationEmojis["Stop"])
            {
                await message.DeleteAsync();
                await context.RespondAsync("Navigation stopped");

                break;
            }
            else if (result.Emoji == NavigationEmojis["Start"] && currentPage < embeds.Count - 1)
            {
                currentPage = 0;

                await message.ModifyAsync(embeds[currentPage]);
            }
            else if (result.Emoji == NavigationEmojis["Backwards"] && currentPage > 0)
            {
                currentPage--;

                await message.ModifyAsync(embeds[currentPage]);
            }

            await message.DeleteReactionAsync(result.Emoji, result.User);
        }
    }

    private static bool ReactionPredicate(CommandContext context, MessageReactionAddEventArgs args)
    {
        var user = args.User;
        var emoji = args.Emoji;

        return context.User == user && NavigationEmojis.ContainsValue(emoji) && context.Channel == args.Channel;
    }

    /// <summary>
    /// This basically is a lazy way to automate `DiscordEmbed` pages
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <returns></returns>
    public static async Task<List<DiscordEmbed>> ToDiscordPages<T>(
        this InteractivityExtension _,
        IEnumerable<T> items,
        Func<T, string> titleSelector,
        Func<T, string>? descriptionSelector = null,
        Func<T, DiscordColor>? colorSelector = null,
        Func<T, (string text, string iconUrl)>? footerSelector = null,
        Func<T, DateTimeOffset>? timestampSelector = null,
        Func<T, Task<string>>? thumbnailSelector = null,
        params Func<T, (string name, string value)>[] fieldSelectors)
    {
        var embedTasks = items.Select(async item =>
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = titleSelector(item)
            };

            if (descriptionSelector is not null)
                embed.WithDescription(descriptionSelector(item));

            if (colorSelector is not null)
                embed.WithColor(colorSelector(item));

            if (footerSelector is not null)
            {
                var (text, iconUrl) = footerSelector(item);
                embed.WithFooter(text, iconUrl);
            }

            if (timestampSelector is not null)
                embed.WithTimestamp(timestampSelector(item));

            if (thumbnailSelector is not null)
                embed.WithThumbnail(await thumbnailSelector(item));

            foreach (var selector in fieldSelectors)
            {
                var (name, value) = selector(item);
                embed.AddField(name, value);
            }

            return embed.Build();
        });

        return [.. await Task.WhenAll(embedTasks)];
    }

}