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

    public static ConfigEntry<bool> m_auto_clean_smelly;
    public static ConfigEntry<bool> m_auto_empty_trashcans;
    public static ConfigEntry<bool> m_auto_kick_cheaters;
    public static ConfigEntry<bool> m_auto_pickup_money;
    public static ConfigEntry<bool> m_auto_pickup_science;
    public static ConfigEntry<bool> m_bottomless_trashbag;
    public static ConfigEntry<bool> m_cars_enabled;
    public static ConfigEntry<float> m_diving_board_panic_chance;
    public static ConfigEntry<float> m_drown_chance;
    public static ConfigEntry<bool> m_equipment_damage_enabled;
    public static ConfigEntry<bool> m_equipment_dirt_enabled;
    public static ConfigEntry<bool> m_every_hotdog_is_right;
    public static ConfigEntry<bool> m_every_lemonade_is_right;
    public static ConfigEntry<bool> m_every_ticket_is_right;
    public static ConfigEntry<bool> m_infinite_patience;
    public static ConfigEntry<bool> m_litter_enabled;
    public static ConfigEntry<bool> m_max_inventory;
    public static ConfigEntry<bool> m_pee_enabled;
    public static ConfigEntry<bool> m_poop_enabled;
    public static ConfigEntry<bool> m_puddles_enabled;
    public static ConfigEntry<bool> m_seagulls_enabled;
    public static ConfigEntry<float> m_visitor_speed_multiplier;

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

        m_auto_clean_smelly = this.create_entry("General", "Auto Clean Smelly Visitors", false, "Set to true to automatically spray smelly visitors when they enter the park (and get achievement/prestige credit for it!)", on_setting_changed);
        m_auto_empty_trashcans = this.create_entry("General", "Auto Empty Trashcans", false, "Set to true to cause all trashcans to automatically empty periodically.", on_setting_changed);
        m_auto_kick_cheaters = this.create_entry("General", "Auto Kick Cheaters", false, "Set to true to automatically kick out unsavory characters (pervs, line jumpers, etc).", on_setting_changed);
        m_auto_pickup_money = this.create_entry("General", "Auto Pickup - Money", false, "Set to true to automatically pick up Money drops.", on_setting_changed);
        m_auto_pickup_science = this.create_entry("General", "Auto Pickup - Science", false, "Set to true to automatically pick up Science drops.", on_setting_changed);
        m_bottomless_trashbag = this.create_entry("General", "Bottomless Trashbag", false, "Set to true to allow trashbags to hold infinite trash.", on_setting_changed);
        m_cars_enabled = this.create_entry("General", "Cars - Enabled", true, "Set to false to remove cars (and the absolutely terrible drivers...) from the streets.", on_setting_changed);
        m_diving_board_panic_chance = this.create_entry("General", "Diving Board Panic Chance", 5f, "Chance (between 0 and 100) of visitor panicking on the diving board and needing a shove.", on_setting_changed);
        m_drown_chance = this.create_entry("General", "Drown Chance", 5f, "Chance (between 0 and 100) of visitor triggering a Drown event in the pool.  Setting this to 100 will certainly get your park on the national news.", on_setting_changed);
        m_equipment_damage_enabled = this.create_entry("General", "Equipment Damage - Enabled", true, "Set to false to disable equipment damage from daily use.", on_setting_changed);
        m_equipment_dirt_enabled = this.create_entry("General", "Equipment Dirt - Enabled", true, "Set to false to disable equipment dirtying from daily use.", on_setting_changed);
        m_every_hotdog_is_right = this.create_entry("General", "Every Hotdog is Right", false, "Set to true to disable the check on the accuracy of hotdog toppings.", on_setting_changed);
        m_every_lemonade_is_right = this.create_entry("General", "Every Lemonade is Right", false, "Set to true to disable the check on the accuracy of lemonade additions.", on_setting_changed);
        m_every_ticket_is_right = this.create_entry("General", "Every Ticket is Right", false, "Set to true to disable the check on the accuracy of tickets.  Visitors will always see the ticket as correct and pay the regular price for all access to the park.", on_setting_changed);
        m_infinite_patience = this.create_entry("General", "Infinite Patience", false, "Set to true to cause visitors to wait in line forever.", on_setting_changed);
        m_litter_enabled = this.create_entry("General", "Litter - Enabled", true, "Set to false to disable visitor littering.", on_setting_changed);
        m_max_inventory = this.create_entry("General", "Max Inventory Size", false, "Set to true to set inventory (i.e. action bar) size to maximum (10).  Changes to this setting require a game reload.", on_setting_changed);
        m_pee_enabled = this.create_entry("General", "Pee - Enabled", true, "Set to false to disable urinating in pools.", on_setting_changed);
        m_poop_enabled = this.create_entry("General", "Poop - Enabled", true, "Set to false to disable duece dropping outside of designated receptacles.", on_setting_changed);
        m_puddles_enabled = this.create_entry("General", "Puddles - Enabled", true, "Set to false to disable the creation of water puddles.", on_setting_changed);
        m_seagulls_enabled = this.create_entry("General", "Seagulls - Enabled", true, "Set to false to disable seagull events.", on_setting_changed);
        m_visitor_speed_multiplier = this.create_entry("General", "Visitor Movement Speed Multiplier", 1f, "Multiplier applied to visitor movement speed (float, default 1f [normal speed], greater than 1 == faster).  Note that values above ~2 will cause physics to get wonky and make people miss their marks and do wacky stuff.", on_setting_changed);
    }

    public void late_load() {
    }

    public static void on_setting_changed(object sender, EventArgs e) {
		
	}
}