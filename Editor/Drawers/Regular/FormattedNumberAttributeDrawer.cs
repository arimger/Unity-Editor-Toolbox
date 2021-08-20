using System;
using System.Globalization;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(FormattedNumberAttribute))]
    public class FormattedNumberAttributeDrawer : PropertyDrawerBase
    {
        private readonly NumberFormatInfo formatInfo = new NumberFormatInfo()
        {
            NumberGroupSeparator = " ",
            CurrencySymbol = "$",
            CurrencyDecimalSeparator = "."
        };


        private void ApplyControlName(string propertyKey)
        {
            GUI.SetNextControlName(propertyKey);
        }

        private bool IsControlEditing(string propertyKey)
        {
            return EditorGUIUtility.editingTextField && GUI.GetNameOfFocusedControl() == propertyKey;
        }

        private Single GetSingle(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Integer
                ? property.intValue
                : property.floatValue;
        }

        private string GetFormat(SerializedProperty property, FormattedNumberAttribute attribute)
        {
            var isInt = property.propertyType == SerializedPropertyType.Integer;
            return string.Format("{0}{1}", attribute.Format, isInt ? 0 : attribute.DecimalsToShow);
        }


        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var key = property.GetPropertyHashKey();
            ApplyControlName(key);
            EditorGUI.PropertyField(position, property, label);
            if (IsControlEditing(key))
            {
                position.width = 0;
                position.height = 0;
            }
            else
            {
                position.xMin += EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
            }

            var targetAttribute = attribute as FormattedNumberAttribute;
            var single = GetSingle(property);
            var format = GetFormat(property, targetAttribute);

            try
            {
                EditorGUI.TextField(position, single.ToString(format, formatInfo));
            }
            catch (FormatException)
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property, string.Format("{0} format is not supported.", format));
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Integer ||
                   property.propertyType == SerializedPropertyType.Float;
        }
    }
}