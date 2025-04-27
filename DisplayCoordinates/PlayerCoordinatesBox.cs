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
            GUILayout.BeginArea(Plugin.layoutRect);
            
            GUILayout.Box(Plugin.name);
            GUILayout.Box("x: " + Plugin.coords.x.ToString("0.00") + "  y: " + Plugin.coords.y.ToString("0.00") + "  z: " + Plugin.coords.z.ToString("0.00"));
            
            GUILayout.EndArea();
        }
    }
}
 