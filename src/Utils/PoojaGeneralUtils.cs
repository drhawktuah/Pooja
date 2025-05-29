using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pooja.src.Exceptions.General;

namespace Pooja.src.Utils;

public static class PoojaGeneralUtils
{
    public static PoojaHierarchy ConvertPoojaHierarchy(string position)
    {
        position = position.ToLower();

        switch (position)
        {
            case "admin":
                {
                    return PoojaHierarchy.Admin;
                }
            case "moderator":
                {
                    return PoojaHierarchy.Moderator;
                }
            case "helper":
                {
                    return PoojaHierarchy.Helper;
                }
            case "intern":
                {
                    return PoojaHierarchy.Intern;
                }
            default:
                {
                    throw new PoojaGeneralException($"{position} is not a valid position");
                }
        }
    }

    public static string ConvertPoojaHierarchy(PoojaHierarchy position)
    {
        switch (position)
        {
            case PoojaHierarchy.Admin:
                {
                    return "Admin";
                }
            case PoojaHierarchy.Moderator:
                {
                    return "Moderator";
                }
            case PoojaHierarchy.Helper:
                {
                    return "Helper";
                }
            case PoojaHierarchy.Intern:
                {
                    return "Intern";
                }
            default:
                return "Unknown";
        }
    }
}