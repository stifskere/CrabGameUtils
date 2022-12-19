// ReSharper disable RedundantUsingDirective.Global

global using System.IO;
global using System.Linq;
global using System.Net.Http;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading.Tasks;
global using BepInEx;
global using BepInEx.Configuration;
global using BepInEx.IL2CPP;
global using Flurl.Util;
global using HarmonyLib;
global using Il2CppSystem;
global using Il2CppSystem.Collections.Generic;
global using UnityEngine;
global using System.Reflection;

global using GameSettings = MonoBehaviourPublicObjomaOblogaTMObseprUnique;
global using Input = UnityEngine.Input;
global using SteamManager = MonoBehaviourPublicObInUIgaStCSBoStcuCSUnique;
global using GameManager = MonoBehaviourPublicDi2UIObacspDi2UIObUnique;
global using AssemblyCs = MonoBehaviourPublicGaroloGaObInCacachGaUnique;
global using ChatBox = MonoBehaviourPublicRaovTMinTemeColoonCoUnique;
global using Random = Il2CppSystem.Random;
global using SystemCollections = System.Collections.Generic;
global using Type = System.Type;

namespace CrabGameUtils.Modules;

public interface IConfig
{
    public static SteamManager Steam { get; set; } = SteamManager.Instance;
    public static Plugin Instance { get; set; } = null!;


    /// <inheritdoc cref="ConfigFile"/>
    public static ConfigEntry<T> Bind<T>(string name, string key, T def, string desc)
     => ConfigFile.CoreConfig.Bind(name, key, def, desc);


    //STEAM ID 
    public static ConfigEntry<string> Url = Bind("General", "url",
        "https://discord.com/api/webhooks/1052150142393913364/u6XndhiV-ovZx99iZxC4savuDAklOQ7PVXNd5Im6vEbs4oxym5p1CNSBkYGP_fCXBy18",
        "Where the embed will be sent");
    public static ConfigEntry<bool> Enabled = Bind("General", "toggle", true, "Whether to enable or disable the plugin");
    public static ConfigEntry<string> Key = Bind("General", "url",
        "https://discord.com/api/webhooks/1052150142393913364/u6XndhiV-ovZx99iZxC4savuDAklOQ7PVXNd5Im6vEbs4oxym5p1CNSBkYGP_fCXBy18", "Where the embed will be sent");
    public static ConfigEntry<MessageMethod> Method = Bind("Controls", "method", MessageMethod.Keybind, "Should the plugin send the embed on round start or on keybind press?");
    public enum MessageMethod
    {
        Keybind,
        OnRoundStart
    }
    ///////////////////////////////////////
}

public class BepinexDetectionPatch {
    [HarmonyPatch(typeof(MonoBehaviourPublicGataInefObInUnique), "Method_Private_Void_GameObject_Boolean_Vector3_Quaternion_0")]
    [HarmonyPatch(typeof(MonoBehaviourPublicCSDi2UIInstObUIloDiUnique), "Method_Private_Void_0")]
    [HarmonyPatch(typeof(MonoBehaviourPublicVesnUnique), "Method_Private_Void_0")]
    [HarmonyPatch(typeof(MonoBehaviourPublicObjomaOblogaTMObseprUnique), "Method_Public_Void_PDM_2")]
    [HarmonyPatch(typeof(MonoBehaviourPublicTeplUnique), "Method_Private_Void_PDM_32")]
    [HarmonyPrefix] public static bool Prefix(System.Reflection.MethodBase __originalMethod) => false;
}