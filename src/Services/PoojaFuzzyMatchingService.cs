using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Pooja.src.Attributes;

namespace Pooja.src.Services;

public sealed class PoojaFuzzyMatchingService
{

    private readonly CommandsNextExtension commandsNext;
    private readonly Dictionary<string, Command> commands = [];

    private readonly bool isAdminOrOwner;

    public PoojaFuzzyMatchingService(CommandsNextExtension commandsNext, bool isAdminOrOwner = false)
    {
        this.commandsNext = commandsNext;
        this.isAdminOrOwner = isAdminOrOwner;

        GetCommands();
    }

    public IEnumerable<PoojaFuzzyResult> GetResults(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentNullException(nameof(query), "cannot be null, empty or whitespace");
        }

        foreach (var command in commands.Values)
        {
            var distance = CalculateDistance(query, command);
            var percentage = CalculatePercentage(distance, Math.Max(command.Name.Length, query.Length));

            var result = new PoojaFuzzyResult
            {
                Command = command,
                ModuleName = command.Module?.ModuleType.Name ?? "no module found...",
                Distance = distance,
                Similarity = $"{percentage:0.##}%",
                Icon = GetColoredPercentage(percentage)
            };

            yield return result;
        }
    }

    public async Task<DiscordEmbed> ToDiscordEmbed(string query, IEnumerable<PoojaFuzzyResult> results)
    {
        var embedBuilder = new DiscordEmbedBuilder()
            .WithColor(0x503dfc);

        var stringBuilder = new StringBuilder();

        if (!results.Any())
        {
            stringBuilder.AppendLine($"no matches for '{query}' have been found");
        }
        else
        {
            embedBuilder.WithTitle($"command '{query}' cannot be found. alternatives:");

            stringBuilder.AppendLine("```");
            foreach (var result in results.Where(r => r.Distance > 1))
            {
                stringBuilder
                    .AppendLine($"  **`'{result.Command.Name}'`** from **`'{result.ModuleName}'`** with a match of **`'{result.Similarity}'`** - {result.Icon}")
                    .AppendLine();
            }

            stringBuilder.AppendLine("```");

            embedBuilder.WithFooter($"emojis indicate the relation between your query and the commands specified", commandsNext.Client.CurrentUser.AvatarUrl);
        }

        embedBuilder.WithThumbnail(commandsNext.Client.CurrentUser.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 1024));
        embedBuilder.WithDescription(stringBuilder.ToString());

        return await Task.FromResult(embedBuilder);
    }

    private static int CalculateDistance(string query, Command command)
    {
        if (query.Length == command.Name.Length)
            return 1;

        int[,] distances = new int[query.Length + 1, command.Name.Length + 1];

        for (int i = 0; i <= query.Length; i++)
            distances[i, 0] = i;

        for (int j = 0; j <= command.Name.Length; j++)
            distances[0, j] = j;

        for (int k = 1; k <= query.Length; k++)
        {
            for (int l = 1; l <= command.Name.Length; l++)
            {
                // int distance = a[k - 1] == b[l - 1] ? 0 : 1;
                // i like the if expression here, no changing
                int distance;
                if (query[k - 1] == command.Name[l - 1])
                {
                    distance = 0;
                }
                else
                {
                    distance = 1;
                }

                var kToLDistance = distances[k - 1, l] + 1;
                var lToKDistance = distances[k, l - 1] + 1;

                var lAndKDistance = distances[k - 1, l - 1] + distance;

                distances[k, l] = Math.Min(Math.Min(kToLDistance, lToKDistance), lAndKDistance);
            }
        }

        return distances[query.Length, command.Name.Length];
    }

    private static double CalculatePercentage(int distance, int maxLength)
    {
        return (1.0 - ((double)distance / maxLength)) * 100;
    }

    private static string GetColoredPercentage(double similarity)
    {
        if (similarity == 0.0)
        {
            return "⚫";
        }
        else if (similarity <= 15.0)
        {
            return "🔴";
        }
        else if (similarity <= 25.0)
        {
            return "🟠";
        }
        else if (similarity <= 50.0)
        {
            return "🟡";
        }
        else if (similarity <= 75.0)
        {
            return "🟢";
        }
        else if (similarity <= 95.0)
        {
            return "✅";
        }

        return "⁉️";
    }

    private void GetCommands()
    {
        foreach (var (name, command) in commandsNext.RegisteredCommands)
        {
            if (command.IsHidden && isAdminOrOwner)
            {
                commands[name] = command;
            }
            else
            {
                continue;
            }
        }
    }
}

public struct PoojaFuzzyResult
{
    public int Distance { get; internal set; }
    public string Icon { get; internal set; }
    public string Similarity { get; internal set; }

    public Command Command { get; internal set; }
    public string? ModuleName { get; internal set; } 
}