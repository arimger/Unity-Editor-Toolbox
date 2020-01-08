using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagAttributeDrawer : ToolboxNativePropertyDrawer
    {
        private const float mininumButtonWidth = 80.0f;
        private const float minimumButtonHeight = 16.0f;
        private const float spacing = 3.0f;


        private int maxButtonsInRow = 1;
    
        private float minButtonsWidth;
        private float additionalHeight;


        private void HandlePopupStyle(Rect position, SerializedProperty property, GUIContent label, object targetObject)
        {
            //get field info value from this property, works even on array elements
            var enumValue = property.GetProperValue(fieldInfo, targetObject) as Enum;

            //begin popup property
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            enumValue = EditorGUI.EnumFlagsField(position, label, enumValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = Convert.ToInt32(enumValue);
            }
            EditorGUI.EndProperty();
        }

        private void HandleButtonStyle(Rect position, SerializedProperty property, GUIContent label, object targetObject)
        {
            //collection of indexes used to store all needed enums
            var buttonsToDisplay = new List<int>();

            //begin property and draw label
            EditorGUI.BeginProperty(position, label, property);
            var buttonPosition = EditorGUI.PrefixLabel(position, label);
            EditorGUI.BeginChangeCheck();

            var propertyType = property.GetProperType(fieldInfo, targetObject);
            var enumValues = Enum.GetValues(propertyType);
            var enumNames = Enum.GetNames(propertyType);
            var enumSum = 0;
            //get all proper enum values except 0(nothing) and ~0(everything)
            for (var i = 0; i < enumValues.Length; i++)
            {
                var value = (int)enumValues.GetValue(i);
                if (value == 0 || value == ~0) continue;
                buttonsToDisplay.Add(i);
                enumSum += value;
            }
            //store current enum value if possible
            var enumValue = property.hasMultipleDifferentValues ? 0 : property.intValue == -1 ? enumSum : property.intValue;

            //in buttons states we will store all toggle values for all buttons + "Everything" and "Nothing"
            var buttonsCount = buttonsToDisplay.Count;
            
            //check how many buttons can we draw in one row
            if (Event.current.type == EventType.Repaint)
            {
                //get maximum count of buttons in row
                maxButtonsInRow = Mathf.Max(1, Mathf.FloorToInt(position.width / mininumButtonWidth));
                minButtonsWidth = (position.width - spacing * (maxButtonsInRow - 1)) / maxButtonsInRow;
            }

            //create "Nothing" button
            buttonPosition.height = minimumButtonHeight;
            buttonPosition.width /= 2;
            buttonPosition.width -= spacing / 2;
            enumValue = GUI.Toggle(buttonPosition, enumValue == 0, "Nothing", Style.toggleStyle) ? 0 : enumValue;
            //create "Everything" button
            buttonPosition.x += buttonPosition.width + spacing;
            enumValue = GUI.Toggle(buttonPosition, enumValue == enumSum, "Everything", Style.toggleStyle) ? enumSum : enumValue;

            //set basic rect for first button
            buttonPosition = position;
            buttonPosition.height = minimumButtonHeight;
            buttonPosition.width = minButtonsWidth;

            //draw all other buttons depending on enum values
            for (var i = 0; i < buttonsCount; i++)
            {
                //row break if we reach limit
                if (i % maxButtonsInRow == 0)
                {
                    buttonPosition.x = position.x;
                    buttonPosition.y += minimumButtonHeight + spacing;
                    additionalHeight += minimumButtonHeight + spacing;
                    //adjust buttons width to row width or use maximum possible width
                    var buttonsInRow = buttonsCount - i;
                    if (buttonsInRow < maxButtonsInRow)
                    {
                        buttonPosition.width = (position.width - spacing * (buttonsInRow - 1)) / buttonsInRow;
                    }
                    else
                    {
                        buttonPosition.width = minButtonsWidth;
                    }
                }

                //cache index and enum value
                var index = buttonsToDisplay[i];
                var value = (int) enumValues.GetValue(index);

                //draw toggle for each enum value
                enumValue = GUI.Toggle(buttonPosition, enumValue == (enumValue | value), enumNames[index], Style.toggleStyle) 
                    ? enumValue | value
                    : enumValue & ~value;
                //adjust position for next button
                buttonPosition.x += buttonPosition.width + spacing;
            }

            if (EditorGUI.EndChangeCheck())
            {
                //-1 value will specify when the whole mask is selected
                property.intValue = enumValue == enumSum ? -1 : enumValue;
            }
            EditorGUI.EndProperty();
        }


        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            additionalHeight = 0;

            switch (Attribute.Style)
            {
                case EnumStyle.Popup:
                    HandlePopupStyle(position, property, label, property.GetDeclaringObject());
                    return;
                case EnumStyle.Button:
                    HandleButtonStyle(position, property, label, property.GetDeclaringObject());
                    return;
                default:
                    HandlePopupStyle(position, property, label, property.GetDeclaringObject());
                    return;
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Enum;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + additionalHeight;
        }

 
        private EnumFlagAttribute Attribute => attribute as EnumFlagAttribute;


        private static class Style
        {
            internal static readonly GUIStyle toggleStyle;

            static Style()
            {
                toggleStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 9
                };
            }
        }
    }
}