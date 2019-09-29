using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(ColorSpaceAttribute))]
    public class ColorSpaceAttributeDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return base.GetHeight() + Attribute.SpaceHeight;
        }

        public override void OnGUI(Rect position)
        {
            var lineX = (position.x + (position.width / 2)) - Attribute.LineWidth / 2;
            var lineY = position.y + (Attribute.SpaceHeight / 2);
            var lineWidth = Attribute.LineWidth;
            var lineHeight = Attribute.LineHeight;

            var oldGuiColor = GUI.color;

            GUI.color = Attribute.LineColor;
            EditorGUI.DrawPreviewTexture(new Rect(lineX, lineY, lineWidth, lineHeight), Texture2D.whiteTexture);
            GUI.color = oldGuiColor;
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="ColorSpaceAttribute"/>.
        /// </summary>
        private ColorSpaceAttribute Attribute => attribute as ColorSpaceAttribute;
    }
}