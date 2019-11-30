using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class HighlightAttributeDrawer : ToolboxDecoratorDrawer<HighlightAttribute>
    {
        protected override void OnGuiBeginSafe(HighlightAttribute attribute)
        {
            Style.SetHighlightColor(attribute.Color);

            EditorGUILayout.BeginVertical(Style.highlightStyle);
        }

        protected override void OnGuiEndSafe(HighlightAttribute attribute)
        {
            EditorGUILayout.EndVertical();
        }


        private static class Style
        {
            internal static readonly GUIStyle highlightStyle = new GUIStyle();

            internal static void SetHighlightColor(Color color)
            {
                var backgroundTex = highlightStyle.normal.background;

                if (highlightStyle.normal.background == null)
                {
                    backgroundTex = new Texture2D(1, 1)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    highlightStyle.normal.background = backgroundTex;
                }

                backgroundTex.SetPixel(0, 0, color);
                backgroundTex.Apply();
            }
        }
    }
}