using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pooja.src.Utils;

public static class EconomyUtils
{
    public static string FormatCurrency(long currency)
    {
        if (currency < 1000)
        {
            return $"{currency}";
        }
        else
        {
            return $"{currency:##,#}";
        }
    }
}