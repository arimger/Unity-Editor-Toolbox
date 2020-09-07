using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class BeginGroupAttributeDrawer : ToolboxDecoratorDrawer<BeginGroupAttribute>
    {
        protected override void OnGuiBeginSafe(BeginGroupAttribute attribute)
        {
            if (attribute.HasLabel)
            {
                using (new GUILayout.HorizontalScope(Style.labelBackgroundStyle))
                {
                    //draw group label for the whole section
                    GUILayout.Label(attribute.Label, Style.labelStyle);
                }

                //remove additional space between layouts
                GUILayout.Space(Style.labelOffset);
            }

            ToolboxLayoutHelper.BeginVertical(Style.groupBackgroundStyle);
        }


        private static class Style
        {
#if UNITY_2019_3_OR_NEWER
            internal static readonly float labelOffset = -3.0f;
#else
            internal static readonly float labelOffset = -5.0f;
#endif
            internal static readonly GUIStyle labelStyle;
            internal static readonly GUIStyle labelBackgroundStyle;
            internal static readonly GUIStyle groupBackgroundStyle;

            static Style()
            {
                labelStyle = new GUIStyle(EditorStyles.boldLabel);
#if UNITY_2019_3_OR_NEWER
                labelBackgroundStyle = new GUIStyle(EditorStyles.helpBox);
                groupBackgroundStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    padding = new RectOffset(13, 12, 5, 5)
                };
#else
                labelBackgroundStyle = new GUIStyle(GUI.skin.box);
                groupBackgroundStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(13, 12, 5, 5)
                };
#endif
            }
        }
    }
}