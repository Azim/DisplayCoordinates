using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace DisplayCoordinates
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        private readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        private static BepInEx.Logging.ManualLogSource mls;
        private static UnityEngine.Vector3 coords;
        private static string name;


        public override void Load()
        {
            harmony.PatchAll(typeof(Plugin));
            mls = Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(PlayerNetwork), "Update")]
        [HarmonyPrefix]
        static void Update(PlayerNetwork __instance)
        {
            if (__instance.NetworkInstantiated) //crude current player check
            {
                //mls.LogInfo("networked player " + __instance.CharacterName);
            }
            else
            {
                //mls.LogInfo("not networked player " + __instance.CharacterName);
                name = __instance.CharacterName;  //to double check that we are indeed getting current player. Maybe useful for a world map later?
                coords = __instance.PlayerSync.PlayerWorldPosition;
            }
        }

        [HarmonyPatch(typeof(BuildInfoHud), "Update")]
        [HarmonyPrefix]
        static void UpdateBuildInfoHud(BuildInfoHud __instance)
        {
            __instance._payload.BuildNumberText = "x: " + coords.x + "  y: " + coords.y + "  z: " + coords.z + "  |  " + name;
        }
    }
}