using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Pooja.src.Services;

namespace Pooja.src.Modules;

public partial class EconomyGamesModule : BaseCommandModule
{
    public required EconomyService EconomyService { get; set; }
}