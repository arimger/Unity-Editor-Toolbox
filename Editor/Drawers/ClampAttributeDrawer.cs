using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ClampAttribute))]
    public class ClampAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only number value properties.");
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
                return;
            }

            var minValue = Attribute.MinValue;
            var maxValue = Attribute.MaxValue;

            if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = Mathf.Clamp(property.floatValue, minValue, maxValue);           
            }
            else
            {
                property.intValue = Mathf.Clamp(property.intValue, (int)minValue, (int)maxValue);
            }

            EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="ClampAttribute"/>.
        /// </summary>
        private ClampAttribute Attribute => attribute as ClampAttribute;
    }
}