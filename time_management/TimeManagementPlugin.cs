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

    public const string TITLE = "Time Management";
    public const string NAME = "time_management";
    public const string SHORT_DESCRIPTION = "Slow down, speed up, or pause time in-game using configurable hotkeys.";
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
            Settings.Instance.early_load(m_instance);
            m_instance.create_nexus_page();
            this.m_harmony.PatchAll();
            logger.LogInfo($"{PluginInfo.GUID} v{PluginInfo.VERSION} loaded.");
        } catch (Exception e) {
            _error_log("** Awake FATAL - " + e);
        }
    }

    [HarmonyPatch(typeof(TimeControl), "TimeScale", MethodType.Getter)]
    class HarmonyPatch_TimeControl_TimeScale {
        private static bool Prefix(ref float __result) {
            if (!Settings.m_enabled.Value) {
                return true;
            }
            __result = (Settings.m_is_time_paused.Value ? 0 : Settings.m_time_scale.Value);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerController), "Update")]
    class HarmonyPatch_PlayerController_Update {
        private static bool Prefix(PlayerController __instance) {
            string info = null;
			if (Input.GetKeyDown(KeyCode.LeftBracket)) {
				info = $"Time Scale = {(Settings.m_time_scale.Value = Mathf.Max(0, Settings.m_time_scale.Value - Settings.m_time_scale_delta.Value)):0.00}";
			} else if (Input.GetKeyDown(KeyCode.RightBracket)) {
                info = $"Time Scale = {(Settings.m_time_scale.Value += Settings.m_time_scale_delta.Value):0.00}";
            } else if (Input.GetKeyDown(KeyCode.Backslash)) {
                info = $"Time is {((Settings.m_is_time_paused.Value = !Settings.m_is_time_paused.Value) ? "PAUSED" : "RUNNING")}";
            }
            if (info != null) {
                GameManager.Instance.NotificationPanel?.DisplayNotification(info, 2);
                _info_log(info);
            }
            return true;
        }
    }
}