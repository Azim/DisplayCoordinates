using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace DisplayCoordinates
{
    /// <summary>
    /// Class that represents a hotkey: key combination + action + press state
    /// </summary>
    public class Hotkey
    {
        public List<Key> keys;
        public Action action;
        public bool pressed; // flag to prevent multiple triggers while the key is held down

        public Hotkey(List<Key> keys, Action action)
        {
            this.keys = keys ?? throw new ArgumentNullException(nameof(keys));
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            this.pressed = false;
        }
    }

    /// <summary>
    /// Static class to parse hotkey strings into lists of Keys, with fallback to default
    /// </summary>
    public static class HotkeyParser
    {
        /// <summary>
        /// Converts a string in the format "LeftCtrl + LeftShift + Delete" to a List<Key>.
        /// If an invalid key is found, returns the default combination.
        /// </summary>
        /// <param name="hotkeyString">Hotkey string</param>
        /// <param name="defaultCombination">Fallback in case of invalid input</param>
        /// <returns>Valid list of Keys</returns>
        public static List<Key> ParseHotkeyString(string hotkeyString, List<Key> defaultCombination)
        {
            if (string.IsNullOrWhiteSpace(hotkeyString))
                return defaultCombination;

            var keys = new List<Key>();
            var parts = hotkeyString.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                string keyString = part.Trim();

                if (Enum.TryParse<Key>(keyString, ignoreCase: true, out Key key))
                    keys.Add(key);
                else
                {
                    Plugin.mls.LogWarning($"HotkeyParser: Invalid key '{keyString}' in string '{hotkeyString}'. Using default.");
                    return defaultCombination;
                }
            }
            return keys.Count > 0 ? keys : defaultCombination;
        }
    }
}
