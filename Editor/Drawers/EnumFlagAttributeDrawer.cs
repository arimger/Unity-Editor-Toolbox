using System;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagAttributeDrawer : PropertyDrawer
    {
        private const float mininumButtonWidth = 60.0f;


        private float additionalHeight;


        private void HandlePopupStyle(Rect position, SerializedProperty property, GUIContent label)
        {
            var enumValue = fieldInfo.GetValue(property.serializedObject.targetObject) as Enum;
            EditorGUI.BeginProperty(position, label, property);
            property.intValue = Convert.ToInt32(EditorGUI.EnumFlagsField(position, label, enumValue));
            EditorGUI.EndProperty();
        }

        private void HandleButtonStyle(Rect position, SerializedProperty property, GUIContent label)
        {
            var enumValue = 0;
            var maxWidth = EditorGUIUtility.labelWidth;
            var maxHeight = EditorGUIUtility.singleLineHeight;
            var buttonsCount = property.enumDisplayNames.Length;
            var buttonsStates = new bool[buttonsCount];

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            for (var i = 0; i < 0; i++)
            {
                for (var j = 0; j < 0; j++)
                {
                    
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = enumValue;
            }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + additionalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            additionalHeight = 0;

            if (property.propertyType != SerializedPropertyType.Enum)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on enum value properties.");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            switch (Attribute.Style)
            {
                case EnumStyle.Popup:
                    HandlePopupStyle(position, property, label);
                    return;
                case EnumStyle.Button:
                    HandleButtonStyle(position, property, label);
                    return;
                default:
                    HandlePopupStyle(position, property, label);
                    return;
            }
        }


        private EnumFlagAttribute Attribute => attribute as EnumFlagAttribute;


        private static class Style
        {
            internal static readonly GUIStyle toggleStyle;

            static Style()
            {
                toggleStyle = new GUIStyle(GUI.skin.button);
            }
        }
    }
}