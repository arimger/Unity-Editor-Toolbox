using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class BeginGroupAttributeDrawer : ToolboxDecoratorDrawer<BeginGroupAttribute>
    {
        private GUIStyle GetStyle(GroupStyle style)
        {
            switch (style)
            {
                default:
                case GroupStyle.Round:
                    return Style.roundGroupBackgroundStyle;
                case GroupStyle.Boxed:
                    return Style.boxedGroupBackgroundStyle;
            }
        }

        protected override void OnGuiBeginSafe(BeginGroupAttribute attribute)
        {
            var backgroundStyle = GetStyle(attribute.Style);
            ToolboxLayoutHandler.BeginVertical(Style.groupStyle, backgroundStyle);

            if (attribute.HasLabel)
            {
                GUILayout.Label(attribute.Label, EditorStyles.boldLabel);
            }
        }

        private static class Style
        {
            internal static readonly GUIStyle groupStyle;
            internal static readonly GUIStyle roundGroupBackgroundStyle;
            internal static readonly GUIStyle boxedGroupBackgroundStyle;

            static Style()
            {
                groupStyle = new GUIStyle()
                {
                    margin = new RectOffset(3, 3, 2, 2),
                    border = new RectOffset(2, 2, 2, 2),
                    padding = new RectOffset(13, 12, 5, 5)
                };
                roundGroupBackgroundStyle = new GUIStyle("helpBox")
                {
                    padding = new RectOffset(13, 12, 5, 5)
                };
                boxedGroupBackgroundStyle = new GUIStyle("box")
                {
                    padding = new RectOffset(13, 12, 5, 5)
                };
            }
        }
    }
}