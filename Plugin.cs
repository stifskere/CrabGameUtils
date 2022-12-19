
using CrabGameUtils.Modules;

namespace CrabGameUtils;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin, IConfig
{
    
    // private static readonly ConfigEntry<string> Url = 
    //     IConfig.Bind("General", "url", "https://discord.com/api/webhooks/1052150142393913364/u6XndhiV-ovZx99iZxC4savuDAklOQ7PVXNd5Im6vEbs4oxym5p1CNSBkYGP_fCXBy18", "Where the embed will be sent");
    // private static readonly ConfigEntry<bool> Enabled = 
    //     IConfig.Bind("General", "toggle", true, "Whether to enable or disable the plugin");
    // private static readonly ConfigEntry<string> Key = 
    //     IConfig.Bind("Controls", "key", "p", "What keybind should the plugin use to send the embed");
    // private static readonly ConfigEntry<Method> MessageMethod = 
    //     IConfig.Bind("Controls", "method", Method.OnRoundStart, "Should the plugin send the embed on round start or on keybind press?");
    //
    // private enum Method
    // {
    //     Keybind,
    //     OnRoundStart
    // }
    private static SystemCollections.List<Extension> ExtensionInstances { get; } = new();
    
    public override void Load()
    {
        IConfig.Instance = this;
        Harmony.CreateAndPatchAll(typeof(Plugin));
        using Harmony harmony = new Harmony("PlayerInfo");
        harmony.PatchAll();
        harmony.PatchAll(typeof(BepinexDetectionPatch));
        
        foreach (Type type in Assembly.GetAssembly(typeof(Extension)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Extension))))
            ExtensionInstances.Add((Extension)System.Activator.CreateInstance(type, null));
        ExtensionInstances.Sort();
    }
    

    [HarmonyPatch(typeof(AssemblyCs), "Start"), HarmonyPostfix]
    public static void Start(AssemblyCs __instance)
    {
        foreach (Extension extension in ExtensionInstances)
            extension.Start();
    }

    [HarmonyPatch(typeof(AssemblyCs), "Update"), HarmonyPostfix]
    public static void Update(AssemblyCs __instance)
    {
        foreach (Extension extension in ExtensionInstances)
            extension.Update();
    }
    
   
    // 
    //
    // private static async Task GetDataAndSendAsync()
    // {
    //     ChatBox.Instance.ForceMessage("Sending server stats...");
    //     
    //     await Task.Delay(System.TimeSpan.FromSeconds(1));
    //     
    //     string descriptionFields = string.Empty;
    //     foreach (KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.activePlayers)
    //         descriptionFields += $"Name: {player.Value.username ?? "username not found."}\nSteamId64: {player.Value.steamProfile.m_SteamID.ToString() ?? "user steam id not found"}\nNumber: #{player.Value.playerNumber.ToString()}\n\n";
    // }
}

public static class CustomMethods
{
    public static uint RandomColor() => (uint)new Random().Next(0x0, 0xFFFFFF);
}
        

