using Newtonsoft.Json;

namespace Pooja.src;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

public class PoojaConfig
{
    [JsonProperty("owner_ids")]
    public List<ulong> OwnerIDs { get; set; }

    [JsonProperty("prefixes")]
    public List<string> Prefixes { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("admin_ids")]
    public List<ulong> AdminIDs { get; set; }

    public static PoojaConfig Deserialize(string @jsonPath)
    {
        if (string.IsNullOrEmpty(jsonPath) || string.IsNullOrWhiteSpace(jsonPath))
        {
            throw new NullReferenceException($"{nameof(jsonPath)} cannot be empty, null or whitespace");
        }

        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException(nameof(jsonPath));
        }

        string data = File.ReadAllText(jsonPath);

        var config = JsonConvert.DeserializeObject<PoojaConfig>(data);

        if (config == null)
        {
            throw new NullReferenceException($"{nameof(config)} is null");
        }

        return config;
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
