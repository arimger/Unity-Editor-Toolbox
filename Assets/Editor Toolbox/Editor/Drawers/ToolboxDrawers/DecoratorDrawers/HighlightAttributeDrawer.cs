using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class HighlightAttributeDrawer : ToolboxDecoratorDrawer<HighlightAttribute>
    {
        protected override void OnGuiBeginSafe(HighlightAttribute attribute)
        {
            Style.SetHighlightColor(attribute.Color);

            //NOTE: Space(0) will force the Horizontal group to keep old indent
            EditorGUILayout.BeginHorizontal(Style.highlightStyle);
            GUILayout.Space(0);
        }

        protected override void OnGuiEndSafe(HighlightAttribute attribute)
        {
            GUILayout.Space(0);
            EditorGUILayout.EndHorizontal();
        }


        private static class Style
        {
            internal static readonly GUIStyle highlightStyle = new GUIStyle();

            internal static void SetHighlightColor(Color color)
            {
                var backgroundTexture = highlightStyle.normal.background;

                if (highlightStyle.normal.background == null)
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