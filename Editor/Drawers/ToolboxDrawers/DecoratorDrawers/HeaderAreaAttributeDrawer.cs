using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class HeaderAreaAttributeDrawer : ToolboxDecoratorDrawer<HeaderAreaAttribute>
    {
        protected override void OnGuiBeginSafe(HeaderAreaAttribute attribute)
        {
            if (attribute.Style == HeaderStyle.Boxed)
            {
                EditorGUILayout.BeginVertical(Style.boxStyle);
                EditorGUILayout.LabelField(attribute.Label, Style.labelStyle);
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUILayout.LabelField(attribute.Label, Style.labelStyle);
        }


        private static class Style
        {
            internal static readonly GUIStyle boxStyle;
            internal static readonly GUIStyle labelStyle;

            static Style()
            {
                boxStyle = new GUIStyle(GUI.skin.box);
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
            }
        }
    }
}