using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;

namespace DisplayCoordinates
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        internal readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        internal static BepInEx.Logging.ManualLogSource mls;
        internal static Vector3 coords;
        internal static string name="Join the world to see coordinates";
        internal static Rect layoutRect;
        internal static int fontSize;


        public override void Load()
        {
            harmony.PatchAll(typeof(Plugin));
            mls = Log;

            int x = Config.Bind<int>("UI", "x", 0, "X position of coordinates box").Value;
            int y = Config.Bind<int>("UI", "y", 0, "Y position of coordinates box").Value;
            int width = Config.Bind<int>("UI", "width", 500, "width of coordinates box").Value;
            int height = Config.Bind<int>("UI", "height", 100, "height of coordinates box").Value;
            Plugin.fontSize = Config.Bind<int>("UI", "fontSize", 24, "Font size of coordinates").Value;
            layoutRect = new Rect(x, y, width, height);

            IL2CPPChainloader.AddUnityComponent<PlayerCoordinatesBox>();
            GameObject obj = new();
            obj.AddComponent<PlayerCoordinatesBox>();
            GameObject.DontDestroyOnLoad(obj);

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(PlayerNetwork), "Update")]
        [HarmonyPrefix]
        static void Update(PlayerNetwork __instance)
        {
            if (!__instance.NetworkInstantiated) //if not remote player
            {
                name = __instance.CharacterName;
                coords = __instance.PlayerSync.PlayerWorldPosition;
            }
        }
    }
}