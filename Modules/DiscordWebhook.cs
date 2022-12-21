
namespace CrabGameUtils.Modules;

public class DiscordWebhook
{
    private string? URL { get; }
    private string? Username { get; }
    private string? AvatarURL { get; }
    
    private readonly HttpClient _client = new();

    public bool IsValid { get; } = true;
    
    public DiscordWebhook(string url, string username, string? avatarURL = null)
    {
        if (!CheckWebhook(url).Result) IsValid = false;
        URL = url;
        Username = username;
        AvatarURL = avatarURL!;
    }

    ~DiscordWebhook() => _client.Dispose();
    
    private async Task<bool> CheckWebhook(string url)
        => (await new HttpClient().GetAsync(url)).IsSuccessStatusCode;

    public async Task SendAsync(string? message = null, Embed[]? embeds = null)
    {
        embeds ??= System.Array.Empty<Embed>();
        if (message == null && embeds.Length == 0) throw new System.Exception("At least a message or embed are required");

        await _client.PostAsync(URL, new StringContent(JsonSerializer.Serialize(
            new MessageObject
            {
                Username = Username,
                Content = message,
                AvatarUrl = AvatarURL,
                Embeds = embeds.Length == 0 ? null : embeds
            }, new JsonSerializerOptions(JsonSerializerDefaults.Web)), Encoding.UTF8, "application/json"));
    }

    private class MessageObject
    {
        [JsonPropertyName("username")] public string? Username { get; set; }
        [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }
        [JsonPropertyName("content")] public string? Content { get; set; }
        [JsonPropertyName("embeds")] public Embed[]? Embeds { get; set; }
    }
}