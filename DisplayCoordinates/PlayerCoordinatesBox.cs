using DisplayCoordinates;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Drawing;
using System;

public class PlayerCoordinatesBox : MonoBehaviour
{
    private List<Hotkey> hotkeys = new List<Hotkey>();

    void Start()
    {
        string showCoordsString = Plugin.toggleHotkey.Value; // Hotkeys

        // Default hotkey if keys defined by users fails
        var showCoordsDefault = new List<Key> { Key.Insert };

        // strings to key list for fallback
        var showCoordsKeys = HotkeyParser.ParseHotkeyString(showCoordsString, showCoordsDefault);

        // Added the hotkey with the command to exec if pressed
        hotkeys.Add(new Hotkey(showCoordsKeys, () =>
        {
            Plugin.showCoords.Value = !Plugin.showCoords.Value; // Toggles the value of showCoords
            Plugin.configFile.Save(); // Saves the .cfg file
            Plugin.mls.LogInfo($"Hotkey pressed! showCoords is now: {Plugin.showCoords.Value} (Frame: {Time.frameCount})");
        }));
    }

    void Update()
    {
        foreach (var hotkey in hotkeys)
        {
            if (IsHotkeyPressed(hotkey))
            {
                if (!hotkey.pressed && Time.time - Plugin.lastActionTime >= Plugin.actionCooldown)
                {
                    Plugin.lastActionTime = Time.time;
                    hotkey.action.Invoke();
                    hotkey.pressed = true;
                }
            }
            else
            {
                if (hotkey.pressed)
                {
                    hotkey.pressed = false;
                    Plugin.mls.LogInfo("Hotkey cleared!");
                }
            }
        }
    }

    private bool IsHotkeyPressed(Hotkey hotkey)
    {
        foreach (var key in hotkey.keys)
        {
            if (!Keyboard.current[key].isPressed)
                return false;
        }

        Key lastKey = hotkey.keys[hotkey.keys.Count - 1];
        return Keyboard.current[lastKey].wasPressedThisFrame;
    }

    void OnGUI()
    {
        if (!Plugin.showCoords.Value) return;

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = Plugin.fontSize;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        GUILayout.BeginArea(Plugin.layoutRect);
        if (Plugin.showCharName.Value)
            GUILayout.Box(Plugin.name, style);
        GUILayout.Box($"x: {Plugin.coords.x:0.00}  y: {Plugin.coords.y:0.00}  z: {Plugin.coords.z:0.00}", style);
        GUILayout.EndArea();
    }
}