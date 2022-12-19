namespace CrabGameUtils.Modules;

public class DiscordWebhook
{
    private string? URL { get; }
    private string? Username { get; }
    private string? AvatarURL { get; }
    
    private readonly HttpClient _client = new();
    
    public DiscordWebhook(string url, string username, string? avatarURL = null)
    {
        if (!CheckWebhook(url).Result) throw new System.Exception("The url is invalid");
        URL = url;
        Username = username;
        AvatarURL = avatarURL!;
    }

    ~DiscordWebhook() => _client.Dispose();
    
    private async Task<bool> CheckWebhook(string url)
        => (await new HttpClient().GetAsync(url)).IsSuccessStatusCode;

    public async Task SendAsync(string? message = null, Embed[]? embeds = null)
    {
        if (message == null || embeds == null || (message == null && embeds.Length == 0)) throw new System.Exception("At least a message or embed are required");
        await _client.PostAsync(URL, new StringContent(JsonSerializer.Serialize(
            new SystemCollections.Dictionary<string, object?>
            {
                {"username", Username},
                {"avatar_url", AvatarURL},
                {"content", message},
                {"embeds", embeds}
            }
            ), Encoding.UTF8, "application/json"));
    }
}