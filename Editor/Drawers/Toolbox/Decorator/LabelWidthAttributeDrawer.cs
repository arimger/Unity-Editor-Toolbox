using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class LabelWidthAttributeDrawer : ToolboxDecoratorDrawer<LabelWidthAttribute>
    {
        private float widthToRestore;

        protected override void OnGuiBeginSafe(LabelWidthAttribute attribute)
        {
            base.OnGuiBeginSafe(attribute);
            widthToRestore = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = attribute.Width;
        }

        protected override void OnGuiCloseSafe(LabelWidthAttribute attribute)
        {
            EditorGUIUtility.labelWidth = widthToRestore;
            base.OnGuiCloseSafe(attribute);
        }
    }
}