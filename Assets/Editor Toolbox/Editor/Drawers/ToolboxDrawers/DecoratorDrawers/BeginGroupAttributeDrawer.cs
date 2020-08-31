using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class BeginGroupAttributeDrawer : ToolboxDecoratorDrawer<BeginGroupAttribute>
    {
        protected override void OnGuiBeginSafe(BeginGroupAttribute attribute)
        {
            if (attribute.HasLabel)
            {
                var groupLabel = attribute.Label;

                EditorGUILayout.BeginHorizontal(Style.labelBackgroundStyle);
                //draw group label for the whole section
                EditorGUILayout.LabelField(groupLabel, Style.labelStyle);
                EditorGUILayout.EndHorizontal();

                //remove additional space between controls
                GUILayout.Space(Style.labelOffset);
            }

            ToolboxLayoutHelper.BeginVertical(Style.groupBackgroundStyle);
        }


        private static class Style
        {
            internal static readonly float labelOffset = -5.0f;

            internal static readonly GUIStyle labelStyle;
            internal static readonly GUIStyle labelBackgroundStyle;
            internal static readonly GUIStyle groupBackgroundStyle;

            static Style()
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
                labelBackgroundStyle = new GUIStyle(GUI.skin.box);
                groupBackgroundStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(13, 12, 5, 5)
                };
            }
        }
    }
}