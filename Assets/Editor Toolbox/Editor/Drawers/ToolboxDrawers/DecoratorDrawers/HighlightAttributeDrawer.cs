using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class HighlightAttributeDrawer : ToolboxDecoratorDrawer<HighlightAttribute>
    {
        protected override void OnGuiBeginSafe(HighlightAttribute attribute)
        {
            Style.SetHighlightColor(attribute.Color);

            EditorGUILayout.BeginHorizontal();
            //NOTE: we have to remove an additional vertical padding 
            GUILayout.Space(-Style.groupPadding);
            EditorGUILayout.BeginVertical(Style.highlightStyle);
        }

        protected override void OnGuiEndSafe(HighlightAttribute attribute)
        {
            EditorGUILayout.EndVertical();
            GUILayout.Space(-Style.groupPadding);
            EditorGUILayout.EndHorizontal();
        }


        private static class Style
        {
            /// <summary>
            /// Additional padding created by the <see cref="EditorGUILayout.VerticalScope"/>.
            /// </summary>
            internal static readonly float groupPadding = 4.0f;

            internal static readonly GUIStyle highlightStyle = new GUIStyle();

            internal static void SetHighlightColor(Color color)
            {
                var backgroundTexture = highlightStyle.normal.background;
                if (backgroundTexture == null)
                {
                    backgroundTexture = new Texture2D(1, 1)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    highlightStyle.normal.background = backgroundTexture;
                }

                backgroundTexture.SetPixel(0, 0, color);
                backgroundTexture.Apply();
            }
        }
    }
}