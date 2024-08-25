using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class BeginHorizontalGroupAttributeDrawer : ToolboxDecoratorDrawer<BeginHorizontalGroupAttribute>
    {
        static BeginHorizontalGroupAttributeDrawer()
        {
            storage = new ControlDataStorage<Vector2>((id, defaultValue) =>
            {
                return defaultValue;
            });
        }

        /// <summary>
        /// Storage used to cache scroll values depending on the given control ID.
        /// </summary>
        private static readonly ControlDataStorage<Vector2> storage;

        private static float lastFetchedWidth = 0.0f;

        private void HandleScrollView(float fixedHeight)
        {
            var controlId = storage.GetControlId();
            var oldScroll = storage.ReturnItem(controlId, Vector2.zero);
            var newScroll = fixedHeight > 0.0f
                ? EditorGUILayout.BeginScrollView(oldScroll, Style.scrollViewGroupStyle, GUILayout.Height(fixedHeight))
                : EditorGUILayout.BeginScrollView(oldScroll, Style.scrollViewGroupStyle);
            storage.AppendItem(controlId, newScroll);
        }

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

        protected override void OnGuiBeginSafe(BeginHorizontalGroupAttribute attribute)
        {
            if (GuiLayoutUtility.TryGetLayoutWidth(out var layoutWidth))
            {
                lastFetchedWidth = layoutWidth;
            }

            var style = GetStyle(attribute.Style);
            ToolboxLayoutHandler.BeginVertical(style);
            if (attribute.HasLabel)
            {
                GUILayout.Label(attribute.Label, EditorStyles.boldLabel);
            }

            EditorGUIUtility.labelWidth = attribute.LabelWidth;
            if (attribute.ControlFieldWidth && attribute.ElementsInLayout > 0)
            {
                var width = lastFetchedWidth;
                width -= attribute.WidthOffset;
                width -= (attribute.LabelWidth + attribute.WidthOffsetPerElement + EditorGUIUtility.standardVerticalSpacing * 4) * attribute.ElementsInLayout;
                width /= attribute.ElementsInLayout;
                EditorGUIUtility.fieldWidth = width;
            }

            HandleScrollView(attribute.Height);
            ToolboxLayoutHandler.BeginHorizontal();
        }

        private static class Style
        {
            internal static readonly GUIStyle roundGroupBackgroundStyle;
            internal static readonly GUIStyle boxedGroupBackgroundStyle;
            internal static readonly GUIStyle scrollViewGroupStyle;

            static Style()
            {
                roundGroupBackgroundStyle = new GUIStyle("helpBox")
                {
                    padding = new RectOffset(13, 12, 5, 5)
                };
                boxedGroupBackgroundStyle = new GUIStyle("box")
                {
                    padding = new RectOffset(13, 12, 5, 5)
                };

                //NOTE: we need to add the right padding to keep foldout-based properties fully visible
                scrollViewGroupStyle = new GUIStyle("scrollView")
                {
                    padding = new RectOffset(13, 8, 2, 2)
                };
            }
        }
    }
}