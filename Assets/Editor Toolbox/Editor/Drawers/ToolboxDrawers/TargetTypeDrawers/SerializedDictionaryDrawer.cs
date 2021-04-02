#if UNITY_2020_1_OR_NEWER
using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class SerializedDictionaryDrawer : ToolboxTargetTypeDrawer
    {
        public override void OnGui(SerializedProperty property, GUIContent label)
        {
            var listProperty = property.FindPropertyRelative("pairs");
            var keyCollision = property.FindPropertyRelative("keyCollision");
            ToolboxEditorGui.DrawToolboxProperty(listProperty, label);
            if (keyCollision.boolValue)
            {
                EditorGUILayout.HelpBox(Style.warningContent.text, MessageType.Warning);
            }
        }

        public override Type GetTargetType()
        {
            return typeof(SerializedDictionary<,>);
        }

        public override bool UseForChildren()
        {
            return false;
        }


        private static class Style
        {
            internal static readonly GUIContent warningContent = new GUIContent("Some keys are not unique, it will break deserialization.");
        }
    }
}
#endif