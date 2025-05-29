using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Pooja.src.Attributes;
using Pooja.src.Services;

namespace Pooja.src.Events;

/*
[DiscordClientEvent]
public static class MessageCreated
{
    public static async Task MainAsync(DiscordClient client, MessageCreateEventArgs args)
    {
        if (args.Author.IsBot)
            return;

        var commandsNext = client.GetCommandsNext();

        var message = args.Message;
        var houseService = commandsNext.Services.GetRequiredService<RandomHouseService>();

        var result = commandsNext.FindCommand(message.Content, out _);

        if (result == null)
        {
            var content = message.Content.ToLowerInvariant();

            if (Regex.IsMatch(content, @"\b(house|vicodin)\b", RegexOptions.IgnoreCase))
            {
                var randomPicture = houseService.GetRandomPicture();

                await using var fs = randomPicture.OpenRead();

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("vi-vi-vicodi-i-in...?")
                    .WithImageUrl($"attachment://{randomPicture.Name}")
                    .WithColor(0xff0000);

                var builder = new DiscordMessageBuilder()
                    .WithEmbed(embed)
                    .AddFile(randomPicture.Name, fs);

                await message.Channel.SendMessageAsync(builder);
            }
        }
    }
}
*/