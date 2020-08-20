using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    internal static class ToolboxEditorUtility
    {
        private static readonly Type inspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");


        internal static bool IsDefaultScriptProperty(SerializedProperty property)
        {
            return IsDefaultScriptPropertyByPath(property.propertyPath);
        }

        internal static bool IsDefaultScriptPropertyByPath(string propertyPath)
        {
            return propertyPath == "m_Script";
        }

        internal static bool IsDefaultScriptPropertyByType(string propertyType)
        {
            return propertyType == "PPtr<MonoScript>";
        }


        internal static bool IsDefaultObjectIcon(string name)
        {
            return name == "GameObject Icon";
        }

        internal static bool IsDefaultPrefabIcon(string name)
        {
            return name == "Prefab Icon";
        }


        /// <summary>
        /// Forces the available InspectorWindow to repaint. 
        /// </summary>
        internal static void RepaintInspector()
        {
            var windows = Resources.FindObjectsOfTypeAll(inspectorType);
            if (windows == null || windows.Length == 0)
            {
                return;
            }

            for (var i = 0; i < windows.Length; i++)
            {
                var window = windows[i] as EditorWindow;
                if (window)
                {
                    window.Repaint();
                }
            }
        }        
    }
}