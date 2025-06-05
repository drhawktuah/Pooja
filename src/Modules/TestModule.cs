using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Pooja.src.Attributes;
using Pooja.src.Services;

namespace Pooja.src.Modules;

public class TestModule : BaseCommandModule
{
    public required PoojaFuzzyMatchingService MatchingService { get; set; }

    [Command("test")]
    [IsOwner]
    public async Task GetTestAsync(CommandContext context, [RemainingText] string name)
    {
    }
}