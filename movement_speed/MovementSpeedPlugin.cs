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

    public const string TITLE = "Movement Speed";
    public const string NAME = "movement_speed";
    public const string SHORT_DESCRIPTION = "Increase player movement speed using a configurable multiplier.";
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

    [HarmonyPatch(typeof(PlayerMovement), "Start")]
    class HarmonyPatch_PlayerMovement_Start {
        private static void Postfix(ref float ___originalMaxSpeed, ref float ___maxSpeed, ref float ___maxWalkSpeed, ref float ___maxRunSpeed) {
            if (!Settings.m_enabled.Value) {
                return;
            }
            ___originalMaxSpeed = ___maxWalkSpeed = ___maxSpeed *= Settings.m_speed_multiplier.Value;
            ___maxRunSpeed = ___maxWalkSpeed * 1.5f;
        }
    }
}