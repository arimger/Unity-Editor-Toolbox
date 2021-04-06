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
            ToolboxEditorGui.DrawDefaultProperty(listProperty, label, null, (p) =>
            {
                if (p.name.Equals("size"))
                {
                    ToolboxEditorGui.DrawDefaultProperty(p);
                    return;
                }

                using (var scope = new EditorGUILayout.HorizontalScope())
                {
                    using (var scope1 = new EditorGUILayout.VerticalScope())
                    {
                        ToolboxEditorGui.DrawDefaultProperty(p);
                    }

                    //TODO: 'remove' button but only for dictionary elements
                }
            });

            //TODO: 'append' button right after dictionary elements
            //TODO: custom styling

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