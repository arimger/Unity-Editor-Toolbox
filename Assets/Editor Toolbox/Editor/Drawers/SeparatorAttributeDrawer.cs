using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SeparatorAttribute))]
    public class SeparatorAttributeDrawer : DecoratorDrawer
    {
        private readonly static Color lineColor = new Color(0.3f, 0.3f, 0.3f);


        private void DrawLine(Rect rect, float thickness, float padding, float spacing)
        {
            rect.y -= spacing / 2;
            rect.y += padding / 2;
            rect.height = thickness;

            EditorGUI.DrawRect(rect, lineColor);
        }


        public override float GetHeight()
        {
            return Attribute.Thickness + Attribute.Padding;
        }

        public override void OnGUI(Rect rect)
        {
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            DrawLine(rect, Attribute.Thickness, Attribute.Padding, spacing);
        }


        private SeparatorAttribute Attribute => attribute as SeparatorAttribute;
    }
}