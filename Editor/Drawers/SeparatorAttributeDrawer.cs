using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SeparatorAttribute))]
    public class SeparatorAttributeDrawer : ToolboxNativeDecoratorDrawer
    {
        private readonly static Color lineColor = new Color(0.3f, 0.3f, 0.3f);


        private void DrawLine(Rect rect, float thickness, float propertyPadding, float propertySpacing)
        {
            //adjust rect to center between properties
            rect.y -= propertySpacing / 2;
            rect.y += propertyPadding / 2;
            //adjust rect to correct tickness
            rect.height = thickness;
            EditorGUI.DrawRect(rect, lineColor);
        }


        public override float GetHeight()
        {
            return Attribute.Thickness + Attribute.Padding;
        }

        public override void OnGUI(Rect rect)
        {
            DrawLine(rect, Attribute.Thickness, Attribute.Padding, EditorGUIUtility.standardVerticalSpacing);
        }


        private SeparatorAttribute Attribute => attribute as SeparatorAttribute;
    }
}