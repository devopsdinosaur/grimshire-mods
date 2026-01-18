using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class PluginInfo {

    public const string TITLE = "Faster Tools";
    public const string NAME = "faster_tools";
    public const string SHORT_DESCRIPTION = "Greatly increases the speed of tool animations.";
	public const string EXTRA_DETAILS = "";

	public const string VERSION = "0.0.1";

    public const string AUTHOR = "devopsdinosaur";
    public const string GAME_TITLE = "Grimshire";
    public const string GAME = "grimshire";
    public const string GUID = AUTHOR + "." + GAME + "." + NAME;
    public const string REPO = GAME + "-mods";

    public static Dictionary<string, string> to_dict() {
        Dictionary<string, string> info = new Dictionary<string, string>();
        foreach (FieldInfo field in typeof(PluginInfo).GetFields((BindingFlags) 0xFFFFFFF)) {
            info[field.Name.ToLower()] = (string) field.GetValue(null);
        }
        return info;
    }
}

[BepInPlugin(PluginInfo.GUID, PluginInfo.TITLE, PluginInfo.VERSION)]
public class TestingPlugin : DDPlugin {
	private static TestingPlugin m_instance = null;
    private Harmony m_harmony = new Harmony(PluginInfo.GUID);
	
	private void Awake() {
        logger = this.Logger;
        try {
			m_instance = this;
            this.m_plugin_info = PluginInfo.to_dict();
            m_instance.create_nexus_page();
            this.m_harmony.PatchAll();
            logger.LogInfo($"{PluginInfo.GUID} v{PluginInfo.VERSION} loaded.");
        } catch (Exception e) {
            _error_log("** Awake FATAL - " + e);
        }
    }

    [HarmonyPatch(typeof(PlayerController), "UseAxe")]
    class HarmonyPatch_AxePlayerState_UseAxe {
        private static bool Prefix(PlayerController __instance, Vector3 pos, int chargeLevel, ref Vector2 ___axePosUsed, ref int ___axeChargeLevel) {
            ___axePosUsed = pos;
            ___axeChargeLevel = chargeLevel;
            __instance.Invoke("DelayedAxe", 0f);
            __instance.PlaySwingAxeSFX(0.1f);
            return false;
        }
    }

    [HarmonyPatch(typeof(HoePlayerState), "UseHoe")]
    class HarmonyPatch_HoePlayerState_UseHoe {
        private static bool Prefix(PlayerController ___playerRef, Vector3 ___pos) {
            ___playerRef.GetAnim().speed = 3f;
			ReflectionUtils.invoke_method(___playerRef, "UseHoe", new object[] { ___pos, 0f });
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerController), "UsePick")]
    class HarmonyPatch_AxePlayerState_UsePick {
        private static bool Prefix(PlayerController __instance, Vector3 pos, int chargeLevel, ref Vector2 ___pickPosUsed, ref int ___pickChargeLevel) {
            ___pickPosUsed = pos;
            ___pickChargeLevel = chargeLevel;
            __instance.Invoke("DelayedPick", 0f);
            return false;
        }
    }

    [HarmonyPatch(typeof(ScythePlayerState), "UseScythe")]
    class HarmonyPatch_ScythePlayerState_UseScythe {
        private static bool Prefix(PlayerController ___playerRef, Vector3 ___pos) {
            ___playerRef.GetAnim().speed = 3f;
            ReflectionUtils.invoke_method(___playerRef, "UseScythe", new object[] { ___pos, 0f });
            return false;
        }
    }
}