using Pooja.src.Services;

namespace Pooja;

public class Program
{
    public async static Task Main(string[] args)
    {
        //GetTotalLines(".");

        src.Pooja pooja = new();
        await pooja.StartAsync();
    }

    public static void GetTotalLines(string @directoryPath)
    {
        int totalLines = 0;

        foreach (var file in Directory.EnumerateFiles(directoryPath, "*.cs", SearchOption.AllDirectories))
        {
            try
            {
                string[] lines = File.ReadAllLines(file);
                totalLines += lines.Length;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file {file}: {ex.Message}");
            }
        }

        Console.WriteLine(totalLines);
    }
}