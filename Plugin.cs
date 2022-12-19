using CrabGameUtils.Modules;



namespace CrabGameUtils;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin, IConfig
{


    private static readonly ConfigEntry<string> Url = 
        IConfig.Bind("General", "url", "https://discord.com/api/webhooks/1052150142393913364/u6XndhiV-ovZx99iZxC4savuDAklOQ7PVXNd5Im6vEbs4oxym5p1CNSBkYGP_fCXBy18", "Where the embed will be sent");
    private static readonly ConfigEntry<bool> Enabled = 
        IConfig.Bind("General", "toggle", true, "Whether to enable or disable the plugin");
    private static readonly ConfigEntry<string> Key = 
        IConfig.Bind("Controls", "key", "p", "What keybind should the plugin use to send the embed");
    private static readonly ConfigEntry<Method> MessageMethod = 
        IConfig.Bind("Controls", "method", Method.OnRoundStart, "Should the plugin send the embed on round start or on keybind press?");
    
    private enum Method
    {
        Keybind,
        OnRoundStart
    }
    public override void Load()
    {
        IConfig.Instance = this;
        Harmony.CreateAndPatchAll(typeof(Plugin));
        using Harmony harmony = new Harmony("PlayerInfo");
        harmony.PatchAll();
        harmony.PatchAll(typeof(BepinexDetectionPatch));
    }

    [HarmonyPatch(typeof(AssemblyCs), "Start"), HarmonyPostfix]
    public static void Start(AssemblyCs __instance)
    {
        ChatBox.Instance.ForceMessage("hola mooon!");
    }

    [HarmonyPatch(typeof(AssemblyCs), "Update"), HarmonyPostfix]
    public static void Update(AssemblyCs __instance)
    {
        if (!Enabled.Value) return;
        IConfig.Steam = SteamManager.Instance;
        if (MessageMethod.Value == Method.Keybind && Input.GetKeyDown(Key.Value)) GetDataAndSendAsync();
    }
    
    // functions!
    
    public static async Task<bool> Get(string url)
        => (await new HttpClient().GetAsync(url)).IsSuccessStatusCode;
    public static async void Post(Discord content, string url)
    {
        using HttpClient client = new HttpClient();
        HttpResponseMessage message = await client.PostAsync(
            url,
            new StringContent(JsonSerializer.Serialize<Discord>(content), Encoding.UTF8, "application/json"));
    }
    
    public static uint RandomColor() => (uint)new Random().Next(0x0, 0xFFFFFF);
    
    private static async Task GetDataAndSendAsync()
    {
        MonoBehaviourPublicRaovTMinTemeColoonCoUnique.Instance.ForceMessage("Sending server stats...");
        await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(1));
        string descriptionFields = string.Empty;
        foreach (KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.activePlayers)
            descriptionFields += $"Name: {player.Value.username ?? "username not found."}\nSteamId64: {player.Value.steamProfile.m_SteamID.ToString() ?? "user steam id not found"}\nNumber: #{player.Value.playerNumber.ToString() ?? "user not found."}\n\n";
            

        Discord message = new Discord
        {
            Username = "steam", 
            Embeds = new EmbedList
            {
                new Embed()
                {
                    Title = "User list",
                    Description = $"**Here is a user list for the game with code:** `{IConfig.Steam.currentLobby.m_SteamID}`\n**This game has:** `{GameManager.Instance.activePlayers.count} players`\n```{descriptionFields}```",
                    EmbedColor = RandomColor(),
                    Fields = new()
                }
            }
        };    

        Post(message, Url.Value);
    }
}

// classes
public class Discord
{
    [JsonPropertyName("content")] public string? Content { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; }
    [JsonPropertyName("embeds")] public EmbedList Embeds { get; set; }
}

public class Embed
{
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("fields")] public FieldList Fields { get; set; }
    [JsonPropertyName("color")] public uint EmbedColor { get; set; }
}

public class EmbedField
{
    [JsonPropertyName("text")] public string Text { get; set; }
    [JsonPropertyName("value")] public string Value { get; set; }
    [JsonPropertyName("inline")] public bool InLine { get; set; }
}
        

