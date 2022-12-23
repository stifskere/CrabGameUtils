
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CrabGameUtils.Modules;

public class DiscordWebhook : WebHookData
{
    private WebhookDebugMode DebugMode { get; }
    
    private readonly HttpClient _client = new();

    public bool IsValid { get; } = true;
    
    public DiscordWebhook(string url, string username, string? avatarURL = null, WebhookDebugMode debug = WebhookDebugMode.None)
    {
        if (!CheckWebhook(url).Result) IsValid = false;
        URL = url;
        Username = username;
        AvatarURL = avatarURL!;
        DebugMode = debug;
    }

    ~DiscordWebhook() => _client.Dispose();
    
    private async Task<bool> CheckWebhook(string url)
    {
        HttpResponseMessage res = await _client.GetAsync(url);
        if (!res.IsSuccessStatusCode) return false;
        WebHookData content = JsonSerializer.Deserialize<WebHookData>(await res.Content.ReadAsStringAsync())!;
        ID = content.ID!;
        ChannelID = content.ChannelID!;
        GuildID = content.GuildID!;
        AppID = content.AppID!;
        Token = content.Token!;
        return true;
    }
    
    public async Task SendAsync(string? message = null, Embed[]? embeds = null, Button[]? components = null)
    {
        embeds ??= System.Array.Empty<Embed>();
        components ??= System.Array.Empty<Button>();
        if (message == null && embeds.Length == 0 && components.Length == 0)
        {
            Instance.Log.LogError("You need to send some content.");
        }

        string content = JsonSerializer.Serialize(
            new MessageObject
            {
                Username = Username,
                Content = message,
                AvatarUrl = AvatarURL,
                Embeds = embeds.Length == 0 ? null : embeds,
                Components = components.Length == 0 ? null : new [] {new Components
                {
                    ComponentsButtons = components
                }}
            }, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        
        HttpResponseMessage res = await _client.PostAsync(URL, new StringContent(content, Encoding.UTF8, "application/json"));
        
        if ((!res.IsSuccessStatusCode && DebugMode == WebhookDebugMode.OnError) || DebugMode == WebhookDebugMode.Always) Instance.Log.LogInfo($"{res}\n{content}");
    }

    private class MessageObject
    {
        [JsonPropertyName("username")] public string? Username { get; set; }
        [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }
        [JsonPropertyName("content")] public string? Content { get; set; }
        [JsonPropertyName("embeds")] public Embed[]? Embeds { get; set; }
        [JsonPropertyName("components")] public Components[]? Components { get; set; }
    }
}

public class WebHookData
{
    [JsonPropertyName("url")] protected string? URL { get; set; }
    [JsonPropertyName("username")] public string? Username { get; set; }
    [JsonPropertyName("avatar_url")] public string? AvatarURL { get; set; }
    [JsonPropertyName("id")] public string? ID { get; protected set; }
    [JsonPropertyName("channel_id")] public string? ChannelID { get; protected set; }
    [JsonPropertyName("guild_id")] public string? GuildID { get; protected set; }
    [JsonPropertyName("application_id")] public string? AppID { get; protected set; }
    [JsonPropertyName("token")] public string? Token { get; protected set; }
}

public enum WebhookDebugMode
{
    None = 0,
    OnError = 1,
    Always = 2
}
