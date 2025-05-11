using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.InputSystem;

namespace DisplayCoordinates
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        internal readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        internal static BepInEx.Logging.ManualLogSource mls;
        internal static Vector3 coords;
        internal static string name = "Join the world to see coordinates";
        internal static Rect layoutRect;
        internal static int fontSize;
        internal static ConfigEntry<bool> showCoords;
        internal static ConfigEntry<string> toggleHotkey;
        internal static ConfigFile configFile;
        internal static float lastActionTime;
        internal const float actionCooldown = 0.5f;
        internal static Key[] modifierKeys;
        internal static Key primaryKey;

        public override void Load()
        {
            harmony.PatchAll(typeof(Plugin));
            mls = Log;

            configFile = Config;
            int x = Config.Bind<int>("UI", "x", 0, "X position of the coordinates box").Value;
            int y = Config.Bind<int>("UI", "y", 0, "Y position of the coordinates box").Value;
            int width = Config.Bind<int>("UI", "width", 250, "Width of the coordinates box").Value;
            int height = Config.Bind<int>("UI", "height", 50, "Height of the coordinates box").Value;
            fontSize = Config.Bind<int>("UI", "fontSize", 14, "Font size of the coordinates text").Value;
            showCoords = Config.Bind<bool>("UI", "showCoords", true, "Whether to show coordinates");
            toggleHotkey = Config.Bind("UI", "toggleHotkey", "Insert", "Use Unity InputSystem Key names (https://docs.unity3d.com/ScriptReference/KeyCode.html), separated by '+'). Examples: RightControl + LeftShift + Insert");
            layoutRect = new Rect(x, y, width, height);

            // Parse hotkeys
            ParseHotkeyConfig();

            IL2CPPChainloader.AddUnityComponent<PlayerCoordinatesBox>();
            GameObject obj = new GameObject("DisplayCoordinatesManager");
            obj.AddComponent<PlayerCoordinatesBox>();
            GameObject.DontDestroyOnLoad(obj);

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        private static void ParseHotkeyConfig()
        {
            try
            {
                string hotkeyString = toggleHotkey.Value.Trim();
                if (string.IsNullOrEmpty(hotkeyString))
                {
                    mls.LogWarning("Hotkey string is empty. Using default: Insert");
                    SetDefaultHotkeys();
                    return;
                }

                // Split string using " + "
                string[] keyNames = hotkeyString.Split(new[] { " + " }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(k => k.Trim())
                                               .ToArray();

                if (keyNames.Length < 1)
                {
                    mls.LogWarning("No valid keys found in hotkey string. Using default: Insert");
                    SetDefaultHotkeys();
                    return;
                }

                // Last key is primary the others are modifiers
                string primaryKeyName = keyNames[keyNames.Length - 1];
                string[] modifierKeyNames = keyNames.Take(keyNames.Length - 1).ToArray();

                // Parse primary key
                if (!Enum.TryParse<Key>(primaryKeyName, true, out Key parsedPrimaryKey))
                {
                    mls.LogWarning($"Invalid primary key '{primaryKeyName}'. Using default: Insert");
                    SetDefaultHotkeys();
                    return;
                }
                primaryKey = parsedPrimaryKey;

                // Parse modifiers keys
                modifierKeys = new Key[modifierKeyNames.Length];
                for (int i = 0; i < modifierKeyNames.Length; i++)
                {
                    if (!Enum.TryParse<Key>(modifierKeyNames[i], true, out Key parsedModifier))
                    {
                        mls.LogWarning($"Invalid modifier key '{modifierKeyNames[i]}'. Using default: Insert");
                        SetDefaultHotkeys();
                        return;
                    }
                    modifierKeys[i] = parsedModifier;
                }

                mls.LogInfo($"Hotkeys parsed successfully: Modifiers=[{string.Join(", ", modifierKeys)}], Primary={primaryKey}");
            }
            catch (Exception ex)
            {
                mls.LogWarning($"Error parsing hotkey string '{toggleHotkey.Value}': {ex.Message}. Using default: Insert");
                SetDefaultHotkeys();
            }
        }

        private static void SetDefaultHotkeys()
        {
            modifierKeys = new Key[] {  };
            primaryKey = Key.Insert;
            toggleHotkey.Value = "Insert";
            configFile.Save();
        }

        [HarmonyPatch(typeof(PlayerNetwork), "Update")]
        [HarmonyPrefix]
        static void Update(PlayerNetwork __instance)
        {
            if (!__instance.NetworkInstantiated)
            {
                name = __instance.CharacterName;
                coords = __instance.PlayerSync.PlayerWorldPosition;
            }
        }
    }
}