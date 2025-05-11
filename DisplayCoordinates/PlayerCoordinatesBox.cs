using DisplayCoordinates;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCoordinatesBox : MonoBehaviour
{
    private bool hotkeyPressed = false; // Flag to check if the key is being held

    void Update()
    {
        // Checks if all modifier keys are pressed
        bool modifiersHeld = AllModifiersHeld();

        // Checks if the primary key was pressed in this frame
        if (modifiersHeld && PrimaryKeyDown() && !hotkeyPressed)
        {
            // Applies cooldown to prevent multiple toggles
            if (Time.time - Plugin.lastActionTime >= Plugin.actionCooldown)
            {
                Plugin.lastActionTime = Time.time;
                Plugin.showCoords.Value = !Plugin.showCoords.Value; // Toggles the value of showCoords
                Plugin.configFile.Save(); // Saves the .cfg file
                Plugin.mls.LogInfo($"Hotkey pressed! showCoords is now: {Plugin.showCoords} (Frame: {Time.frameCount})");
                hotkeyPressed = true; // Marks that the key is being held
            }
        }
        // Resets the state when the primary key is released
        else if (hotkeyPressed && !Keyboard.current[Plugin.primaryKey].isPressed)
        {
            hotkeyPressed = false;
            Plugin.mls.LogInfo("Hotkey liberada!");
        }
    }

    bool AllModifiersHeld()
    {
        bool allModifiersPressed = true;
        foreach (var modifier in Plugin.modifierKeys)
        {
            if (!Keyboard.current[modifier].isPressed)
            {
                allModifiersPressed = false;
                break;
            }
        }
        return allModifiersPressed;
    }

    bool PrimaryKeyDown()
    {
        // Checks if the primary key was pressed in this frame
        bool pressed = Keyboard.current[Plugin.primaryKey].wasPressedThisFrame;
        if (pressed)
            Plugin.mls.LogInfo($"PrimaryKeyDown detected for {Plugin.primaryKey} (Frame: {Time.frameCount})");
        return pressed;
    }

    void OnGUI()
    {
        if (!Plugin.showCoords.Value) return;

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = Plugin.fontSize;        
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleLeft;
        GUILayout.BeginArea(Plugin.layoutRect);
        GUILayout.Box(Plugin.name, style);
        GUILayout.Box($"x: {Plugin.coords.x:0.00}  y: {Plugin.coords.y:0.00}  z: {Plugin.coords.z:0.00}", style);
        GUILayout.EndArea();
    }
}