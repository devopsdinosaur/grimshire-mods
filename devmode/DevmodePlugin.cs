using BepInEx;
using BepInEx.Configuration;
using Febucci.UI;
using HarmonyLib;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class PluginInfo {

    public const string TITLE = "Dev Mode";
    public const string NAME = "devmode";
    public const string SHORT_DESCRIPTION = "Enable developer mode for spawning items using the '=' key and click on the bottom right of pause menu to bring up the cheats window!";
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
public class DevmodePlugin : DDPlugin {
	private static DevmodePlugin m_instance = null;
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
	
	[HarmonyPatch(typeof(GameManager), "IsDevPC")]
	class HarmonyPatch_GameManager_IsDevPC {
		private static bool Prefix(ref bool __result) {
			_info_log("Enabling dev mode.  Hit '=' key to bring up item spawning window.  Click on the bottom right of pause menu to bring up the cheats window.");
			__result = true;
			return false;
		}
	}

}