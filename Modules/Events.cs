// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace CrabGameUtils.Modules;

public class Events
{
    public delegate void PlayerActionDelegate(ulong id);
    public static event PlayerActionDelegate RemovePlayerEvent = null!;
    public static event PlayerActionDelegate RespawnPlayerEvent = null!;
    public static event PlayerActionDelegate SpawnPlayerEvent = null!;
    
    public delegate void ChatBoxActionDelegate(string text);
    public static event ChatBoxActionDelegate ChatBoxSubmitEvent = null!;

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
        => ChatBoxSubmitEvent?.Invoke(__0);
    
}