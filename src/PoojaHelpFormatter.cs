using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using Pooja.src.Extensions;

namespace Pooja.src;


public sealed partial class PoojaHelpFormatter : BaseHelpFormatter
{
    public DiscordEmbedBuilder EmbedBuilder { get; }

    private readonly CommandContext ctx;

    private Command? command;

    public PoojaHelpFormatter(CommandContext ctx) : base(ctx)
    {
        this.ctx = ctx;

        EmbedBuilder = new()
        {
            Color = EmbedColor
        };
    }

    public override BaseHelpFormatter WithCommand(Command command)
    {
        this.command = command;

        FormatDescription(command);

        FormatAliases(command);
        FormatCommand(command);
        FormatOverloads(command);

        return this;
    }

    public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
    {
        foreach (var cmd in subcommands.DistinctBy(c => c.Name))
        {
            EmbedBuilder.AddField($"{ctx.Prefix}{cmd.QualifiedName}", $"`{cmd.Description ?? "no help provided..."}`");
        }

        return this;
    }

    public override CommandHelpMessage Build()
    {
        if (command is null)
        {
            EmbedBuilder.WithDescription("showing all available commands");
            EmbedBuilder.WithFooter("this help command is your friend. this will aid you in whatever you're about to do", ctx.Client.CurrentUser.AvatarUrl);
        }
        else
        {
            EmbedBuilder.WithFooter("'[]' signifies command arguments. if your command has those braces, you need to specify arguments for that specific command");
        }

        EmbedBuilder.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl);

        return new CommandHelpMessage(embed: EmbedBuilder.Build());
    }
}

public sealed partial class PoojaHelpFormatter
{
    public static DiscordColor EmbedColor => new(0x6354f0);

    public void FormatCommand(Command command)
    {
        EmbedBuilder.Title = command.QualifiedName;
    }

    private void FormatDescription(Command command)
    {
        StringBuilder builder = new();

        builder.AppendLine((command.Description is null || command.Description.Length == 0) ? "`no help provided...`" : $"`{command.Description}`");

        EmbedBuilder.WithDescription(builder.ToString());
    }

    private void FormatOverloads(Command command)
    {
        StringBuilder builder = new();

        if (command.Overloads.Count > 0)
        {
            foreach (CommandOverload? overload in command.Overloads.OrderByDescending(x => x.Priority))
            {
                string arguments;

                if (overload.Arguments.Count > 0)
                {
                    arguments = string.Join(' ', overload.Arguments.Select(a => $"`[{a.Name}]`"));
                }
                else
                {
                    arguments = "`no arguments found...`";
                }

                builder.AppendLine($"`{ctx.Prefix}{command.QualifiedName}` {arguments}");
            }
        }

        EmbedBuilder.AddField("arguments", builder.ToString().Trim(), false);
    }

    private void FormatAliases(Command command)
    {
        EmbedBuilder.AddField("aliases", command.Aliases.Count > 0 ? string.Join(", ", command.Aliases.OrderByDescending(x => $"`{x}`")) : "`no aliases found...`", false);
    }
}


/*
public sealed partial class PoojaHelpFormatter : BaseHelpFormatter
{
    private readonly CommandContext context;
    private readonly List<DiscordEmbedBuilder> pages = [];

    private static readonly DiscordColor color = new(0x462dfc);

    private Command? command;

    public PoojaHelpFormatter(CommandContext context) : base(context)
    {
        this.context = context;
    }

    public override BaseHelpFormatter WithCommand(Command command)
    {
        this.command = command;

        var embedBuilder = new DiscordEmbedBuilder();

        FormatCommand(command, embedBuilder);
        FormatDescription(command, embedBuilder);
        FormatAliases(command, embedBuilder);
        FormatOverloads(command, embedBuilder);

        pages.Add(embedBuilder);

        return this;
    }

    public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
    {
        var modules = subcommands
            .Where(c => !c.IsHidden)
            .GroupBy(c => c.Module?.ModuleType.Name ?? "miscellaneous");

        foreach (var module in modules)
        {
            var embedBuilder = new DiscordEmbedBuilder();
            var commands = module.Select(c => c);

            foreach (var command in commands.DistinctBy(c => c.Name))
            {
                embedBuilder.AddField($"`{context.Prefix}{command.QualifiedName}`", $"`{command.Description ?? "no help provided..."}`");
            }

            pages.Add(embedBuilder);
        }

        return this;
    }

    public override CommandHelpMessage Build()
    {
        if (pages.Count == 0)
        {
            return new CommandHelpMessage("`no help available...`");
        }

        Task.Run(async () =>
        {
            var interactivity = context.Client.GetInteractivity();
            var interactivityPages = pages
                .Select(embed => new Page(embed: embed))
                .ToList();

            await interactivity.SendPaginatedMessageAsync(
                channel: context.Channel,
                user: context.User,
                pages: interactivityPages,
                behaviour: PaginationBehaviour.Ignore,
                buttons: new PaginationButtons(),
                deletion: ButtonPaginationBehavior.DeleteButtons,
                timeoutoverride: TimeSpan.FromMinutes(2)
            );
        });

        return new CommandHelpMessage();
    }
}


public sealed partial class PoojaHelpFormatter
{
    public static DiscordColor EmbedColor => new(0x6354f0);

    public void FormatCommand(Command command, DiscordEmbedBuilder embedBuilder)
    {
        embedBuilder.WithTitle(command.Name);
    }

    private void FormatDescription(Command command, DiscordEmbedBuilder embedBuilder)
    {
        StringBuilder builder = new();

        builder.AppendLine((command.Description is null || command.Description.Length == 0) ? "`no help provided...`" : $"`{command.Description}`");

        embedBuilder.WithDescription(builder.ToString());
    }

    private void FormatOverloads(Command command, DiscordEmbedBuilder embedBuilder)
    {
        StringBuilder stringBuilder = new();

        if (command.Overloads.Count > 0)
        {
            embedBuilder = new DiscordEmbedBuilder();

            foreach (CommandOverload? overload in command.Overloads.OrderByDescending(x => x.Priority))
            {
                string arguments;

                if (overload.Arguments.Count > 0)
                {
                    arguments = string.Join(' ', overload.Arguments.Select(a => $"`[{a.Name}]`"));
                }
                else
                {
                    arguments = "`no arguments found...`";
                }

                stringBuilder.AppendLine($"`{context.Prefix}{command.QualifiedName}` {arguments}");
            }

            embedBuilder.AddField("arguments", stringBuilder.ToString().Trim(), false);
            embedBuilder.WithFooter("'[]' signifies command arguments. if your command has those braces, you need to specify arguments for that specific command");
        }
    }

    private void FormatAliases(Command command, DiscordEmbedBuilder embedBuilder)
    {
        embedBuilder.AddField("aliases", command.Aliases.Count > 0 ? string.Join(", ", command.Aliases.OrderByDescending(x => $"`{x}`")) : "`no aliases found...`", false);
    }
}
*/