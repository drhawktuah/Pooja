using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pooja.src.Extensions;

public static partial class StringExtensions
{
    private readonly static Dictionary<char, char> flipped = new()
    {
        ['a'] = 'ɐ',
        ['b'] = 'q',
        ['c'] = 'ɔ',
        ['d'] = 'p',
        ['e'] = 'ǝ',
        ['f'] = 'ɟ',
        ['g'] = 'ƃ',
        ['h'] = 'ɥ',
        ['i'] = 'ᴉ',
        ['j'] = 'ɾ',
        ['k'] = 'ʞ',
        ['l'] = 'l',
        ['m'] = 'ɯ',
        ['n'] = 'u',
        ['o'] = 'o',
        ['p'] = 'd',
        ['q'] = 'b',
        ['r'] = 'ɹ',
        ['s'] = 's',
        ['t'] = 'ʇ',
        ['u'] = 'n',
        ['v'] = 'ʌ',
        ['w'] = 'ʍ',
        ['x'] = 'x',
        ['y'] = 'ʎ',
        ['z'] = 'z',
        ['0'] = '0',
        ['1'] = 'Ɩ',
        ['2'] = 'ᄅ',
        ['3'] = 'Ɛ',
        ['4'] = 'ㄣ',
        ['5'] = 'ϛ',
        ['6'] = '9',
        ['7'] = 'ㄥ',
        ['8'] = '8',
        ['9'] = '6'
    };

    /// <summary>
    /// Masks a string by creating a new string with the length of the original string
    /// Since strings in C# are immutable, we had to solve for that by making a new string with the exact same length
    /// of the original string
    /// It also converts it to a codeblock
    /// </summary>
    /// <param name="value"> The original string </param>
    /// <param name="mask"> The character to mask the characters with </param>
    /// <returns> The masked string </returns>
    public static string MaskString(this string value, char mask = '*')
    {
        if (string.IsNullOrEmpty(value))
            return "empty"; // if string is empty, return "empty" as a string

        return '`' + new string(mask, value.Length) + '`'; // you know what this is
    }

    public static string Ordinal(this long integer)
    {
        string s = integer.ToString();

        if (integer < 1)
            return s;

        integer %= 100;
        if ((integer >= 11) && (integer <= 13))
        {
            return s + "th";
        }

        return (integer % 10) switch
        {
            1 => s + "st",
            2 => s + "nd",
            3 => s + "rd",
            _ => s + "th",
        };
    }

    /// <summary>
    /// Capitalizes letters when necessary. Since this isn't built-in and requires a different library, I made my own equivalent with other utilities
    /// </summary>
    /// <param name="input"></param>
    /// <returns>The new string</returns>
    public static string CapitalizeString(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        input = input.Trim();

        StringBuilder builder = new(input);
        bool capitalizeNext = true;

        for (int i = 0; i < builder.Length; i++)
        {
            char character = builder[i];
            if (character == '.')
            {
                capitalizeNext = true;
            }
            else if (capitalizeNext && char.IsLetter(character))
            {
                builder[i] = char.ToUpper(character);

                capitalizeNext = false;
            }
        }

        return builder.ToString();
    }

    public static string FlipString(this string text)
    {
        string flippedText = "";

        for (int i = 0; i < text.Length; i++)
        {
            if (flipped.TryGetValue(text[i], out char flippedChar))
            {
                flippedText += flippedChar;
            }
            else
            {
                flippedText += text[i];
            }
        }

        return flippedText;
    }
}
