
namespace CrabGameUtils.Extensions;

[ExtensionName("Send Steam ID")]
public class SendSteamId : Extension
{
    private DiscordWebhook _webhook = null!;
    
    public ExtensionConfig<string> URL = new("url", "Your webhook", "Where the embed will be sent");
    public ExtensionConfig<string> Key = new("key", "P", "What keybind should the plugin use to send the embed");
    public ExtensionConfig<Method> MessageMethod = new("method", Method.Keybind, "Should the plugin send the embed on round start or on keybind press?");
   
    public new ExtensionConfig<bool> Enabled = new("toggle", false, "Whether to enable or disable the plugin");

    public override void Start()
    {
        _webhook = new DiscordWebhook(URL.Value, "SteamIdSender", debug: WebhookDebugMode.OnError);

        if (!System.Enum.TryParse(Key.Value, out KeyCode _))
        {
            ThrowError("SteamIdSender Error, the keybind is not valid.");
            return;
        }

        if (URL.Value == "Your webhook")
        {
            ThrowError("SteamIdSender Error, you need to change the webhook in your config file.");
            return;
        }
        
        if (!_webhook.IsValid)
        {
            ThrowError("SteamIdSender Error, the webhook is not valid.");
            return;
        }
        
        if (MessageMethod.Value == Method.OnRoundStart)
        {
            ChatBox.Instance.ForceMessage("SteamIdSender Loaded, will send members info each round start.");
            _ = GetDataAndSendAsync();
            return;
        }
        
        ChatBox.Instance.ForceMessage($"<color=#00FFFF>SteamIdSender Loaded, press {Key.Value} to send members info.</color>");
    }

    public override void Update()
    {
        if (Input.GetKeyDown(System.Enum.Parse<UnityEngine.KeyCode>(Key.Value)) && MessageMethod.Value == Method.Keybind && !ChatBox.Instance.inputField.isFocused)
        {
            _ = GetDataAndSendAsync();
        }
    }

    private async Task GetDataAndSendAsync()
    {
        ChatBox.Instance.ForceMessage("<color=#00FFFF>Sending server stats...</color>");

        string descriptionFields = string.Empty;
        foreach (KeyValuePair<ulong, CPlayer> player in GameManager.Instance.activePlayers)
            descriptionFields += $"\u001b[33mName\u001b[0m\u001b[30m:\u001b[0m \u001b[32m{player.Value.username ?? "username not found."}\u001b[0m\n\u001b[33mSteamID64\u001b[0m\u001b[30m:\u001b[0m \u001b[32m{player.Value.steamProfile.m_SteamID.ToString() ?? "user steam id not found"}\u001b[0m\n\u001b[33mNumber\u001b[0m\u001b[30m:\u001b[0m \u001b[32m #{player.Value.playerNumber.ToString()}\u001b[0m\n\n";
        foreach (KeyValuePair<ulong, CPlayer> player in GameManager.Instance.spectators)
            descriptionFields += $"\u001b[33mName\u001b[0m\u001b[30m:\u001b[0m \u001b[32m{player.Value.username}\u001b[0m\n\u001b[33mSteamID64\u001b[0m\u001b[30m:\u001b[0m \u001b[32m{player.Value.steamProfile.m_SteamID.ToString() ?? "user steam id not found"}\u001b[0m\n\u001b[33mNumber\u001b[0m\u001b[30m:\u001b[0m \u001b[31m Spectator \u001b[0m\n\n";

        EmbedBuilder embed = new EmbedBuilder()
            .SetTitle("User list")
            .SetDescription($"**Here is a list of players for the game with code:** `{Steam.currentLobby.m_SteamID}`\n**This game has:** `{GameManager.Instance.activePlayers.Count + GameManager.Instance.spectators.Count} players`\n```ansi\n{descriptionFields}```")
            .SetColor(RandomColor());

        await _webhook.SendAsync(embeds: new[] { embed.Build() });
    }
    
    public enum Method
    {
        Keybind,
        OnRoundStart
    }
}