using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AnimationCurveSettingsAttribute))]
    public class AnimationCurveSettingsAttributeDrawer : PropertyDrawerBase
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = Attribute;
            var curveRanges = new Rect(
                attribute.Min.x,
                attribute.Min.y,
                attribute.Max.x - attribute.Min.x,
                attribute.Max.y - attribute.Min.y);

            var color = attribute.Color;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.CurveField(position, property, color, curveRanges, label);
            EditorGUI.EndProperty();
        }

        public override bool IsPropertyValid(SerializedProperty property)
        {
            return base.IsPropertyValid(property) && property.propertyType == SerializedPropertyType.AnimationCurve;
        }

        private AnimationCurveSettingsAttribute Attribute => attribute as AnimationCurveSettingsAttribute;
    }
}