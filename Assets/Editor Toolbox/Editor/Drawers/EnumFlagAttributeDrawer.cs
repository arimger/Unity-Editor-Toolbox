using System;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on enum value properties.");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var enumValue = (Enum)fieldInfo.GetValue(property.serializedObject.targetObject);
            EditorGUI.BeginProperty(position, label, property);
            property.intValue = Convert.ToInt32(EditorGUI.EnumFlagsField(position, label, enumValue));
            EditorGUI.EndProperty();
        }
    }
}