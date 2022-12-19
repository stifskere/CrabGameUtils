using CrabGameUtils.Modules;

namespace CrabGameUtils;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin, IConfig
{
    private static readonly ConfigEntry<string> Test = 
        IConfig.Bind("text to say", "Main", "Hello World!", "The text you will see in the chat when you spawn");

    public override void Load()
    {
        IConfig.Instance = this;
        Harmony.CreateAndPatchAll(typeof(Plugin));
        using Harmony harmony = new Harmony("PlayerInfo");
        harmony.PatchAll();
        harmony.PatchAll(typeof(BepinexDetectionPatch));
    }

    [HarmonyPatch(typeof(AssemblyCs), "Start"), HarmonyPostfix]
    public static void Start()
    {
        ChatBox.Instance.ForceMessage(Test.Value);
    }

    [HarmonyPatch(typeof(AssemblyCs), "Update"), HarmonyPostfix]
    public static void Update()
    {
        
    }
}