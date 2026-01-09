using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Settings {
    private static Settings m_instance = null;
    public static Settings Instance {
        get {
            if (m_instance == null) {
                m_instance = new Settings();
            }
            return m_instance;
        }
    }
    private DDPlugin m_plugin = null;
    
    // General
    public static ConfigEntry<bool> m_enabled;
    public static ConfigEntry<string> m_log_level;

    public static ConfigEntry<float> m_time_scale;
    public static ConfigEntry<float> m_time_scale_delta;
    public static ConfigEntry<bool> m_is_time_paused;

    public ConfigEntry<T> create_entry<T>(string category, string name, T default_value, string description, EventHandler change_callback) {
        ConfigEntry<T> result = this.m_plugin.Config.Bind<T>(category, name, default_value, description);
        if (change_callback != null) {
            result.SettingChanged += change_callback;
        }
        return result;
    }

    public void early_load(DDPlugin plugin) {
        this.m_plugin = plugin;
        
        // General
        m_enabled = this.create_entry("General", "Enabled", true, "Set to false to disable this mod.", on_setting_changed);
        m_log_level = this.create_entry("General", "Log Level", "info", "[Advanced] Logging level, one of: 'none' (no logging), 'error' (only errors), 'warn' (errors and warnings), 'info' (normal logging), 'debug' (extra log messages for debugging issues).  Not case sensitive [string, default info].  Debug level not recommended unless you're noticing issues with the mod.  Changes to this setting require an application restart.", on_setting_changed);
        DDPlugin.set_log_level(m_log_level.Value);
        m_time_scale = this.create_entry("Time", "Time Scale", 1f, "Current time scale (float, default 1f [game default normal time]).", on_setting_changed);
        m_time_scale_delta = this.create_entry("Time", "Time Scale Delta", 0.1f, "Change in time scale with each hotkey keypress (float, default 0.1f).", on_setting_changed);
        m_is_time_paused = this.create_entry("Time", "Time Paused", false, "This represents the current time pause state.  It will be toggled in-game with the pause hotkey.", on_setting_changed);
    }

    public void late_load() {
    }

    public static void on_setting_changed(object sender, EventArgs e) {
		
	}
}