using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class BeginHorizontalAttributeDrawer : ToolboxDecoratorDrawer<BeginHorizontalAttribute>
    {
        private static float lastFetchedWidth = 0.0f;

        protected override void OnGuiBeginSafe(BeginHorizontalAttribute attribute)
        {
            if (GuiLayoutUtility.TryGetLayoutWidth(out var layoutWidth))
            {
                lastFetchedWidth = layoutWidth;
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

            ToolboxLayoutHandler.BeginHorizontal();
        }
    }
}