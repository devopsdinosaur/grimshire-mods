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

    public const string TITLE = "Max Stacks";
    public const string NAME = "maxstacks";
    public const string SHORT_DESCRIPTION = "Sets stack size of all stackable items to 99999.";
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

    [HarmonyPatch(typeof(InventoryItem), "StackSize", MethodType.Getter)]
    class HarmonyPatch_InventoryItem_StackSize {
        private static bool Prefix(ref int __result, int ___stackSize) {
            __result = (___stackSize > 1 ? 99999 : ___stackSize);
            return false;
        }
    }
}