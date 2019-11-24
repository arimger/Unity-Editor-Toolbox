using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    internal static class ToolboxEditorUtility
    {
        internal static GUIStyle headerTextStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);

        internal const string defaultScriptPropertyPath = "m_Script";

        internal const string defaultScriptPropertyType = "PPtr<MonoScript>";

        internal const string defaultObjectPickEventName = "ObjectSelectorUpdated";


        internal static bool IsDefaultScriptProperty(SerializedProperty property)
        {
            return defaultScriptPropertyPath == property.propertyPath;
        }

        internal static bool IsDefaultScriptPropertyByPath(string propertyPath)
        {
            return defaultScriptPropertyPath == propertyPath;
        }

        internal static bool IsDefaultScriptPropertyByType(string propertyType)
        {
            return defaultScriptPropertyType == propertyType;
        }
    }
}