using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class HighlightAttributeDrawer : ToolboxDecoratorDrawer<HighlightAttribute>
    {
        private void SetHighlightColor(Color color)
        {
            var backgroundTexture = Style.highlightStyle.normal.background;
            if (backgroundTexture == null)
            {
                backgroundTexture = new Texture2D(1, 1)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                Style.highlightStyle.normal.background = backgroundTexture;
            }

            backgroundTexture.SetPixel(0, 0, color);
            backgroundTexture.Apply();
        }


        protected override void OnGuiBeginSafe(HighlightAttribute attribute)
        {
            SetHighlightColor(attribute.Color);

            //TODO: dedicated method to handle all similar cases
            EditorGUILayout.BeginHorizontal();
            //we have to remove an additional vertical padding
            //to keep proper indentation, in this case we are
            //going to create a horizontal group and space(-4.0f)
            GUILayout.Space(Style.fixedPadding);
            EditorGUILayout.BeginVertical(Style.highlightStyle);
        }

        protected override void OnGuiCloseSafe(HighlightAttribute attribute)
        {
            EditorGUILayout.EndVertical();
            GUILayout.Space(Style.fixedPadding);
            EditorGUILayout.EndHorizontal();
        }


        private static class Style
        {
            internal static readonly float fixedPadding = -4.0f;

            internal static readonly GUIStyle highlightStyle = new GUIStyle();
        }
    }
}