
using CrabGameUtils.Modules;
using Attribute = System.Attribute;

namespace CrabGameUtils;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{ 
    public static SteamManager Steam { get; set; } = SteamManager.Instance;
    public static Plugin Instance { get; set; } = null!;
    public static ConfigFile StaticConfig { get; set; } = null!;
    
    private static SystemCollections.List<Extension> ExtensionInstances { get; } = new();
    
    public override void Load()
    {
        StaticConfig = Config;
        Instance = this;
        Harmony.CreateAndPatchAll(typeof(Plugin));
        using Harmony harmony = new Harmony("PlayerInfo");
        harmony.PatchAll();
        harmony.PatchAll(typeof(BepinexDetectionPatch));
        
        foreach (Type type in Assembly.GetAssembly(typeof(Extension)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Extension))))
        {
            string? name = type.GetCustomAttributesData().First(a => a.AttributeType.Name == "ExtensionNameAttribute").ConstructorArguments.First().Value?.ToString();
            Extension instance = (Extension)System.Activator.CreateInstance(type, null);
            ExtensionInstances.Add(instance);
            foreach (FieldInfo field in type.GetFields().Where(p => p.FieldType.Name.Contains("ExtensionConfig")))
            {
                field.GetValue(instance).GetType().GetMethod("InitConfig")!.Invoke(field.GetValue(instance), new object[] { name ?? type.Name });
            }
        }
        ExtensionInstances.Sort();
    }
    

    [HarmonyPatch(typeof(AssemblyCs), "Start"), HarmonyPostfix]
    public static void Start(AssemblyCs __instance)
    {
        Steam = SteamManager.Instance;
        foreach (Extension extension in ExtensionInstances)
            extension.Start();
    }

    [HarmonyPatch(typeof(AssemblyCs), "Update"), HarmonyPostfix]
    public static void Update(AssemblyCs __instance)
    {
        Steam = SteamManager.Instance;
        foreach (Extension extension in ExtensionInstances)
            extension.Update();
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
        

