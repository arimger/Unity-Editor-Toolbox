using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class HighlightAttributeDrawer : ToolboxDecoratorDrawer<HighlightAttribute>
    {
        private static void SetBackgroundColor(Color color)
        {
            var backgroundTexture = Style.layoutStyle.normal.background;
            if (backgroundTexture == null)
            {
                backgroundTexture = new Texture2D(1, 1)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                Style.layoutStyle.normal.background = backgroundTexture;
            }

            backgroundTexture.SetPixel(0, 0, color);
            backgroundTexture.Apply();
        }


        protected override void OnGuiBeginSafe(HighlightAttribute attribute)
        {
            SetBackgroundColor(attribute.Color);

            GuiLayoutUtility.BeginFixedVertical(Style.layoutStyle);
        }

        protected override void OnGuiCloseSafe(HighlightAttribute attribute)
        {
            GuiLayoutUtility.CloseFixedVertical();
        }


        private static class Style
        {
            internal static readonly GUIStyle layoutStyle = new GUIStyle();
        }
    }
}