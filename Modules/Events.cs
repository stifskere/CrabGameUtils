// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace CrabGameUtils.Modules;

[HarmonyPatch]
public class Events
{
    public delegate void PlayerActionDelegate(ulong id);
    public static event PlayerActionDelegate RemovePlayerEvent = null!;
    public static event PlayerActionDelegate RespawnPlayerEvent = null!;
    public static event PlayerActionDelegate SpawnPlayerEvent = null!;
    
    public delegate void ChatBoxActionDelegate(string text);
    public static event ChatBoxActionDelegate ChatBoxSubmitEvent = null!;

    public delegate void PlayerDiedDelegate(ulong victim, ulong killer, Vector3 position);

    public static event PlayerDiedDelegate PlayerDiedEvent = null!;
    
    public delegate void GameUIStartDelegate(GameUI gameUI);

    public static event GameUIStartDelegate GameUIStartEvent = null!;
    

    [HarmonyPatch(typeof(GameManager), "RemovePlayer"), HarmonyPostfix]
    public static void RemovePlayer(GameManager __instance, ulong __0)
        => RemovePlayerEvent?.Invoke(__0);

    [HarmonyPatch(typeof(GameManager), "PlayerDied"), HarmonyPostfix]
    public static void PlayerDied(GameManager __instance, ulong __0, ulong __1, Vector3 __2)
        => PlayerDiedEvent?.Invoke(__0, __1, __2);
    
    [HarmonyPatch(typeof(GameManager), "SpawnPlayer"), HarmonyPostfix]
    public static void RespawnPlayer(GameManager __instance, ulong __0)
        => SpawnPlayerEvent?.Invoke(__0);
    
    [HarmonyPatch(typeof(GameManager), "RespawnPlayer"), HarmonyPostfix]
    public static void SpawnPlayer(GameManager __instance, ulong __0)
        => RespawnPlayerEvent?.Invoke(__0);

    [HarmonyPatch(typeof(ChatBox), "SendMessage"), HarmonyPostfix]
    public static void SendMessage(ChatBox __instance, string __0)
        => ChatBoxSubmitEvent?.Invoke(__0);

    [HarmonyPatch(typeof(GameUI), nameof(GameUI.Start)), HarmonyPostfix]
    public static void GameUIStart(GameUI __instance)
        => GameUIStartEvent?.Invoke(__instance);
    
    
}