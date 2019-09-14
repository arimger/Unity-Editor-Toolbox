using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class BeginGroupAttributeDrawer : ToolboxAreaDrawer<BeginGroupAttribute>
    {
        public override void OnGuiBegin(BeginGroupAttribute attribute)
        {
            if (attribute.Label != null)
            {
                EditorGUILayout.BeginHorizontal(Style.headerBackgroundStyle);
                EditorGUILayout.LabelField(attribute.Label, Style.headerStyle);
                EditorGUILayout.EndHorizontal();
                //remove additional space between header and vertical group
                GUILayout.Space(-Style.spacing * 2);
            }

            EditorGUILayout.BeginVertical(Style.sectionBackgroundStyle);
            //add additional space between vertical group and first property
            GUILayout.Space(Style.spacing * 2);
        }


        private static class Style
        {
            internal static readonly float spacing = 2.5f;

            internal static GUIStyle headerStyle;
            internal static GUIStyle headerBackgroundStyle;
            internal static GUIStyle sectionBackgroundStyle;
            internal static GUIStyle foldoutStyle;

            static Style()
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel);
                headerBackgroundStyle = new GUIStyle(GUI.skin.box);
                sectionBackgroundStyle = new GUIStyle(GUI.skin.box);
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };
            }
        }
    }
}