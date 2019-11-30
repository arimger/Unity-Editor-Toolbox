using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class BeginGroupAttributeDrawer : ToolboxDecoratorDrawer<BeginGroupAttribute>
    {
        protected override void OnGuiBeginSafe(BeginGroupAttribute attribute)
        {
            if (!string.IsNullOrEmpty(attribute.Label))
            {
                //draw group label for whole section
                EditorGUILayout.BeginHorizontal(Style.headerBackgroundStyle);
                EditorGUILayout.LabelField(attribute.Label, Style.headerStyle);
                EditorGUILayout.EndHorizontal();
                //remove additional space between header and vertical group
                GUILayout.Space(-Style.spacing * 2);
            }

            EditorGUILayout.BeginVertical(Style.sectionBackgroundStyle);
        }


        private static class Style
        {
            internal static readonly float spacing = 2.5f;

            internal static readonly GUIStyle headerStyle;
            internal static readonly GUIStyle headerBackgroundStyle;
            internal static readonly GUIStyle sectionBackgroundStyle;
            internal static readonly GUIStyle foldoutStyle;

            static Style()
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel);
                headerBackgroundStyle = new GUIStyle(GUI.skin.box);
                sectionBackgroundStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(13, 12, 5, 5)
                };
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };
            }
        }
    }
}