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

    public const string TITLE = "Testing";
    public const string NAME = "testing";
    public const string SHORT_DESCRIPTION = "For testing only";
	public const string EXTRA_DETAILS = "";

	public const string VERSION = "0.0.0";

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
			__result = 0.5f;
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerController), "Update")]
    class HarmonyPatch_1 {
        private static bool Prefix(PlayerController __instance) {
            __instance.hasUnlimitedStamina = true;
			if (Input.GetKeyDown(KeyCode.LeftBracket)) {
				GameManager.Instance.NotificationPanel?.DisplayNotification("Hi there!", 5);
			}
            return true;
        }
    }

    [HarmonyPatch(typeof(GameManager), "Awake")]
    class HarmonyPatch_GameManager_Awake {
        private static void Postfix() {
            try {
                foreach (InventoryItem item in Resources.FindObjectsOfTypeAll<InventoryItem>()) {
                    item.decayRate = 0f;
                }
            } catch (Exception e) {
                _error_log("** HarmonyPatch_GameManager_Awake.Postfix ERROR - " + e);
            }
        }
    }

    [HarmonyPatch(typeof(InventoryItem), "StackSize", MethodType.Getter)]
	class HarmonyPatch_InventoryItem_StackSize {
		private static bool Prefix(ref int __result) {
			__result = 99999;
			return false;
		}
	}

	[HarmonyPatch(typeof(PlayerMovement), "Start")]
	class HarmonyPatch_PlayerMovement_Start {
		private static void Postfix(ref float ___originalMaxSpeed, ref float ___maxSpeed, ref float ___maxWalkSpeed, ref float ___maxRunSpeed) {
			___originalMaxSpeed = ___maxWalkSpeed = ___maxSpeed *= 2f;
			___maxRunSpeed = ___maxWalkSpeed * 1.5f;
		}
	}

	/*
	[HarmonyPatch(typeof(), "")]
	class HarmonyPatch_ {
		private static bool Prefix() {
			
			return true;
		}
	}

	[HarmonyPatch(typeof(), "")]
	class HarmonyPatch_ {
		private static void Postfix() {
			
		}
	}

	[HarmonyPatch(typeof(), "")]
	class HarmonyPatch_ {
		private static bool Prefix() {
			try {

				return false;
			} catch (Exception e) {
				_error_log("** XXXXX.Prefix ERROR - " + e);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(), "")]
	class HarmonyPatch_ {
		private static void Postfix() {
			try {
				
			} catch (Exception e) {
				_error_log("** XXXXX.Postfix ERROR - " + e);
			}
		}
	}
	*/
}