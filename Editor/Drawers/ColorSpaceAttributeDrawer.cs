using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(ColorSpaceAttribute))]
    public class ColorSpaceAttributeDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return base.GetHeight() + ColorSpaceAttribute.SpaceHeight;
        }

        public override void OnGUI(Rect position)
        {
            var lineX = (position.x + (position.width / 2)) - ColorSpaceAttribute.LineWidth / 2;
            var lineY = position.y + (ColorSpaceAttribute.SpaceHeight / 2);
            var lineWidth = ColorSpaceAttribute.LineWidth;
            var lineHeight = ColorSpaceAttribute.LineHeight;

            var oldGuiColor = GUI.color;

            GUI.color = ColorSpaceAttribute.LineColor;
            EditorGUI.DrawPreviewTexture(new Rect(lineX, lineY, lineWidth, lineHeight), Texture2D.whiteTexture);
            GUI.color = oldGuiColor;
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="global::ColorSpaceAttribute"/>.
        /// </summary>
        private ColorSpaceAttribute ColorSpaceAttribute => attribute as ColorSpaceAttribute;
    }
}