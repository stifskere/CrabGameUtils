using CrabGameUtils.Modules;

namespace CrabGameUtils.Extensions;

public class SendSteamId : Extension
{
    private DiscordWebhook _webhook = null!;
    
    public ExtensionConfig<string> URL => new("url", "https://discord.com/api/webhooks/1052150142393913364/u6XndhiV-ovZx99iZxC4savuDAklOQ7PVXNd5Im6vEbs4oxym5p1CNSBkYGP_fCXBy18", "Where the embed will be sent");
    public ExtensionConfig<bool> Enabled => new("toggle", true, "Whether to enable or disable the plugin");
    public ExtensionConfig<string> Key => new("key", "p", "What keybind should the plugin use to send the embed");
    public ExtensionConfig<Method> MessageMethod => new("method", Method.Keybind, "Should the plugin send the embed on round start or on keybind press?");
    
    public override void Start()
    {
        if (!Enabled.Value) return;
        ChatBox.Instance.ForceMessage($"SteamIdSender Loaded, press {Key} to send members info.");
        
        _webhook = new DiscordWebhook(URL.Value, "SteamIdSender");
        
        if (MessageMethod.Value == Method.OnRoundStart)
        {
            GetDataAndSendAsync().Wait();
        }
    }

    public override void Update()
    {
        if (!Enabled.Value) return;
        
        if (Input.GetKeyDown(Key.Value) && MessageMethod.Value == Method.Keybind)
        {
            GetDataAndSendAsync().Wait();
        }
    }

    private async Task GetDataAndSendAsync()
    {
        ChatBox.Instance.ForceMessage("Sending server stats...");
        
        await Task.Delay(System.TimeSpan.FromSeconds(1));
        
        string descriptionFields = string.Empty;
        foreach (KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.activePlayers)
            descriptionFields += $"Name: {player.Value.username ?? "username not found."}\nSteamId64: {player.Value.steamProfile.m_SteamID.ToString() ?? "user steam id not found"}\nNumber: #{player.Value.playerNumber.ToString()}\n\n";

        EmbedBuilder embed = new EmbedBuilder()
            .SetTitle("User list")
            .SetDescription($"**Here is a list of players for the game with code:** `{Steam.currentLobby.m_SteamID}`\n**This game has:** `{GameManager.Instance.activePlayers.Count} players`\n{descriptionFields}")
            .SetColor(RandomColor());

        await _webhook.SendAsync(embeds: new []{embed.Build()});
    }
    
    public enum Method
    {
        Keybind,
        OnRoundStart
    }
}