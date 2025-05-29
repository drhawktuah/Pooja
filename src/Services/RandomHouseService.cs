using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Pooja.src.Services;

/// <summary>
/// All things done the Gregory House way
/// </summary>
public sealed class RandomHouseService
{
    private readonly FileInfo[] images;
    private readonly string[] quotes;

    public RandomHouseService(string imageDirectory, string quotesFilePath)
    {
        if (!Directory.Exists(imageDirectory))
            throw new DirectoryNotFoundException($"Image directory not found: {imageDirectory}");

        if (!File.Exists(quotesFilePath))
            throw new FileNotFoundException($"Quotes file not found: {quotesFilePath}");

        images = LoadImageFiles(imageDirectory);
        quotes = LoadQuotes(quotesFilePath);

        if (images.Length == 0)
            throw new InvalidOperationException("No image files found in the specified directory");

        if (quotes.Length == 0)
            throw new InvalidOperationException("No quotes found in the specified file");
    }

    public FileInfo GetRandomPicture()
    {
        int index = RandomNumberGenerator.GetInt32(images.Length);
        return images[index];
    }

    public string GetRandomQuote()
    {
        int index = RandomNumberGenerator.GetInt32(quotes.Length);
        return quotes[index];
    }

    private static FileInfo[] LoadImageFiles(string imageDirectory)
    {
        return [.. Directory
            .EnumerateFiles(imageDirectory, "*.*", SearchOption.TopDirectoryOnly)
            .Where(IsImageFile)
            .Select(f => new FileInfo(f))];
    }

    private static string[] LoadQuotes(string path)
    {
        return [.. File
            .ReadAllLines(path)
            .Where(line => !string.IsNullOrWhiteSpace(line))];
    }

    private static bool IsImageFile(string path)
    {
        string ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif";
    }
}