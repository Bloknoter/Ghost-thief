using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CuriousInspectorSettings
{
    public class SettingsWindow : EditorWindow
    {
        [MenuItem("Curious Inspector/Settings")]
        private static void GetWindow()
        {
            GetWindow<SettingsWindow>();
        }

        private string[] tabs = new string[] { "General", "Debug" };
        private int currtab;

        private void OnGUI()
        {
            currtab = GUILayout.Toolbar(currtab, tabs);
            switch (currtab)
            {
                case 0:
                    break;
                case 1:
                    SettingsData.GetSettingsData().InspectorDebug = EditorGUILayout.Toggle("Inspector debugging", SettingsData.GetSettingsData().InspectorDebug);
                    SettingsData.GetSettingsData().ContainersDebug = EditorGUILayout.Toggle("Containers debugging", SettingsData.GetSettingsData().ContainersDebug);
                    SettingsData.GetSettingsData().DrawDebug = EditorGUILayout.Toggle("Drawing debugging", SettingsData.GetSettingsData().DrawDebug);
                    break;
            }
        }
    }
}
