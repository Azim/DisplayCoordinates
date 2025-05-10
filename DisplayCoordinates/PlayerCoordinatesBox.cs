using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DisplayCoordinates
{
    internal class PlayerCoordinatesBox : UnityEngine.MonoBehaviour
    {
        void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = Plugin.fontSize; // Ajuste este valor para aumentar ou diminuir a fonte
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.textColor = UnityEngine.Color.white;

            GUILayout.BeginArea(Plugin.layoutRect);
            GUILayout.Box(Plugin.name, style);
            GUILayout.Box($"x: {Plugin.coords.x:0.00}  y: {Plugin.coords.y:0.00}  z: {Plugin.coords.z:0.00}", style);
            GUILayout.EndArea();
        }
    }
}
 