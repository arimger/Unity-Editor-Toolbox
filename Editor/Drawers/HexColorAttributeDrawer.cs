using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(HexColorAttribute))]
    public class HexColorAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            const string hexFormat = "#{0}";

            //begin property drawing
            label = EditorGUI.BeginProperty(position, label, property);
            //create the associated label
            position = EditorGUI.PrefixLabel(position, label);

            var color = Color.white;
            ColorUtility.TryParseHtmlString(string.Format(hexFormat, property.stringValue), out color);
        
            //prepare proper rects for each field
            var labelRect = position;
            labelRect.xMax -= position.width / 2;
            var colorRect = position;
            colorRect.xMin += position.width / 2;
          
            //draw the color field and the associated label
            color = EditorGUI.ColorField(colorRect, GUIContent.none, color, true, true, false);
            property.stringValue = ColorUtility.ToHtmlStringRGBA(color);
            EditorGUI.SelectableLabel(labelRect, string.Format(hexFormat, property.stringValue));
            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}
