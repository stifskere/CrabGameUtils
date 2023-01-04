
using Exception = System.Exception;

namespace CrabGameUtils;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
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
            string name = type.GetCustomAttributesData().First(a => a.AttributeType.Name == "ExtensionNameAttribute").ConstructorArguments.First().Value?.ToString() ?? type.Name;
            Extension instance = (Extension)System.Activator.CreateInstance(type, null);
            foreach (FieldInfo field in type.GetFields().Where(p => p.FieldType.Name.Contains("ExtensionConfig")))
                field.GetValue(instance).GetType().GetMethod("InitConfig")!.Invoke(field.GetValue(instance), new object[] { name });
            if (!instance.Enabled.Value) continue;
            ExtensionInstances.Add(instance);
            instance.Name = name;
            Instance.Log.LogInfo($"{name}: loaded successfully");
        }
    }
    
    [HarmonyPatch(typeof(GameUI), "Awake"), HarmonyPostfix]
    public static void Awake(GameUI __instance)
    {
        Steam = SteamManager.Instance;
        foreach (Extension extension in ExtensionInstances) extension.Awake();
    }

    [HarmonyPatch(typeof(GameUI), "Start"), HarmonyPostfix]
    public static void Start(GameUI __instance)
    {
        Steam = SteamManager.Instance;
        foreach (Extension extension in ExtensionInstances)
        {
            try { extension.Start(); }
            catch (Exception e)
            {
                extension.ThrowError($"{extension.Name} errored (see logs for more details)");
                Instance.Log.LogError(e.ToString());
            }
        }
    }

    [HarmonyPatch(typeof(GameUI), "Update"), HarmonyPostfix]
    public static void Update(GameUI __instance)
    {
        Steam = SteamManager.Instance;
        foreach (Extension extension in ExtensionInstances)
        {
            try { extension.Update(); }
            catch (Exception e)
            {
                extension.ThrowError($"{extension.Name} errored (see logs for more details)");
                Instance.Log.LogError(e.ToString());
            }
        }
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

