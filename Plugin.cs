
namespace CrabGameUtils;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public delegate void PlayerActionDelegate(ulong id);
    public static event PlayerActionDelegate RemovePlayerEvent = null!;
    public static event PlayerActionDelegate RespawnPlayerEvent = null!;
    public static event PlayerActionDelegate SpawnPlayerEvent = null!;

    public delegate void ChatBoxActionDelegate(string text);

    public static event ChatBoxActionDelegate ChatBoxSubmitEvent = null!;
    
    public static SteamManager Steam { get; set; } = SteamManager.Instance;
    public static Plugin Instance { get; set; } = null!;
    public static ConfigFile StaticConfig { get; set; } = null!;
    
    public static SystemCollections.List<Extension> ExtensionInstances { get; } = new();
    
    public override void Load()
    {
        StaticConfig = Config;
        Instance = this;
        Harmony.CreateAndPatchAll(typeof(Plugin));
        Harmony harmony = new Harmony("CrabGameUtils");
        harmony.PatchAll();
        harmony.PatchAll(typeof(BepinexDetectionPatch));
        
        foreach (Type type in Assembly.GetAssembly(typeof(Extension)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Extension))))
        {
            string? name = type.GetCustomAttributesData().First(a => a.AttributeType.Name == "ExtensionNameAttribute").ConstructorArguments.First().Value?.ToString();
            Extension instance = (Extension)System.Activator.CreateInstance(type, null);
            foreach (FieldInfo field in type.GetFields().Where(p => p.FieldType.Name.Contains("ExtensionConfig")))
                field.GetValue(instance).GetType().GetMethod("InitConfig")!.Invoke(field.GetValue(instance), new object[] { name ?? type.Name });
            if (!instance.Enabled.Value) continue;
            ExtensionInstances.Add(instance);
        }
    }
    

    [HarmonyPatch(typeof(GameUI), "Start"), HarmonyPostfix]
    public static void Start(GameUI __instance)
    {
        Steam = SteamManager.Instance;
        foreach (Extension extension in ExtensionInstances) extension.Start();
    }

    [HarmonyPatch(typeof(GameUI), "Update"), HarmonyPostfix]
    public static void Update(GameUI __instance)
    {
        Steam = SteamManager.Instance;
        foreach (Extension extension in ExtensionInstances) extension.Update();
    }

    [HarmonyPatch(typeof(GameManager), "RemovePlayer"), HarmonyPostfix]
    public static void RemovePlayer(GameManager __instance, ulong __0)
        => RemovePlayerEvent?.Invoke(__0);
    
    [HarmonyPatch(typeof(GameManager), "SpawnPlayer"), HarmonyPostfix]
    public static void RespawnPlayer(GameManager __instance, ulong __0)
        => SpawnPlayerEvent?.Invoke(__0);
    
    [HarmonyPatch(typeof(GameManager), "RespawnPlayer"), HarmonyPostfix]
    public static void SpawnPlayer(GameManager __instance, ulong __0)
        => RespawnPlayerEvent?.Invoke(__0);

    [HarmonyPatch(typeof(ChatBox), "SendMessage"), HarmonyPostfix]
    public static void SendMessage(ChatBox __instance, string __0)
    {
        ChatBoxSubmitEvent?.Invoke(__0);
    }
}

public class BepinexDetectionPatch {
    [HarmonyPatch(typeof(MonoBehaviourPublicGataInefObInUnique), "Method_Private_Void_GameObject_Boolean_Vector3_Quaternion_0")]
    [HarmonyPatch(typeof(MonoBehaviourPublicCSDi2UIInstObUIloDiUnique), "Method_Private_Void_0")]
    [HarmonyPatch(typeof(MonoBehaviourPublicVesnUnique), "Method_Private_Void_0")]
    [HarmonyPatch(typeof(MonoBehaviourPublicObjomaOblogaTMObseprUnique), "Method_Public_Void_PDM_2")]
    [HarmonyPatch(typeof(MonoBehaviourPublicTeplUnique), "Method_Private_Void_PDM_32")]
    [HarmonyPrefix] public static bool Prefix(MethodBase __originalMethod) => false;
}

public static class CustomMethods
{
    public static uint RandomColor() => (uint)new Random().Next(0x0, 0xFFFFFF);
}

