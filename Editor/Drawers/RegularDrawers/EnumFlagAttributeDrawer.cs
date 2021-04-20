using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagAttributeDrawer : ToolboxNativePropertyDrawer
    {
        private int maxButtonsInLine = 1;

        private float passableMinWidth;
        private float additionalHeight;
        private float singleLineHeight;

        private object declaringObject;


        private void DrawDefaultGui(Rect position, SerializedProperty property, GUIContent label, object targetObject)
        {
            //get field info value from this property, works even on array elements
            var enumValue = property.GetProperValue(fieldInfo, targetObject) as Enum;

            //begin the true property
            label = EditorGUI.BeginProperty(position, label, property);
            //draw the prefix label
            position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.BeginChangeCheck();
            enumValue = EditorGUI.EnumFlagsField(position, GUIContent.none, enumValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = Convert.ToInt32(enumValue);
            }

            EditorGUI.EndProperty();
        }

        private void DrawButtonsGui(Rect position, SerializedProperty property, GUIContent label, object targetObject)
        {
            //begin property and draw label
            label = EditorGUI.BeginProperty(position, label, property);
            var buttonPosition = EditorGUI.PrefixLabel(position, label);

            var propertyType = property.GetProperType(fieldInfo, targetObject);

            var enumValues = Enum.GetValues(propertyType);
            var enumNames = Enum.GetNames(propertyType);
            var enumSum = 0;

            var attribute = (EnumFlagAttribute)this.attribute;
            var buttonsSpacing = attribute.ButtonsSpacing;
            var buttonsHeight = attribute.ButtonsHeight;
            var buttonsWidth = attribute.ButtonsWidth;

            //collection of indexes used to store all needed labels
            var buttonsToDisplay = new List<int>();
            //get all proper enum values except 0 and ~0(everything)
            for (var i = 0; i < enumValues.Length; i++)
            {
                var value = (int)enumValues.GetValue(i);
                if (value == 0 || value == ~0)
                {
                    continue;
                }

                buttonsToDisplay.Add(i);
                enumSum += value;
            }

            //check how many buttons can we draw in one row
            if (Event.current.type == EventType.Repaint)
            {
                //get maximum count of buttons in a single row
                maxButtonsInLine = Mathf.Max(1, Mathf.FloorToInt(position.width / buttonsWidth));
                passableMinWidth = (position.width - buttonsSpacing * (maxButtonsInLine - 1)) / maxButtonsInLine;
            }

            const int nothing = 0;
            //store current enum value if possible
            var enumValue = property.intValue == -1 ? enumSum : property.intValue;

            buttonPosition.height = buttonsHeight;
            buttonPosition.width /= 2;
            buttonPosition.width -= buttonsSpacing / 2;

            EditorGUI.BeginChangeCheck();
            //create "Nothing" and "Everything" buttons
            var toggleValue = enumValue == nothing;
            var toggleLabel = "Nothing";
            //validate value for the multiple selection
            if (property.hasMultipleDifferentValues)
            {
                enumValue = 0;
            }

            enumValue = GUI.Toggle(buttonPosition, toggleValue, toggleLabel, Style.buttonStyle) ? nothing : enumValue;
            buttonPosition.x += buttonPosition.width + buttonsSpacing;

            toggleValue = enumValue == enumSum;
            toggleLabel = "Everything";
            enumValue = GUI.Toggle(buttonPosition, toggleValue, toggleLabel, Style.buttonStyle) ? enumSum : enumValue;

            //adjust additional height to the first row
            additionalHeight = buttonsHeight > singleLineHeight ? buttonsHeight - singleLineHeight : 0.0f;

            var buttonsCount = buttonsToDisplay.Count;
            //draw all buttons depending on enum values
            for (var i = 0; i < buttonsCount; i++)
            {
                //row break if we reach limit
                if (i % maxButtonsInLine == 0)
                {
                    buttonPosition.x = position.x;
                    buttonPosition.y += buttonsHeight + buttonsSpacing;
                    additionalHeight += buttonsHeight + buttonsSpacing;
                    //adjust buttons width to row width or use maximum possible width
                    var buttonsInRow = buttonsCount - i;
                    if (buttonsInRow < maxButtonsInLine)
                    {
                        buttonPosition.width = (position.width - buttonsSpacing * (buttonsInRow - 1)) / buttonsInRow;
                    }
                    else
                    {
                        buttonPosition.width = passableMinWidth;
                    }
                }

                //cache index and valid enum value
                var index = buttonsToDisplay[i];
                var value = (int)enumValues.GetValue(index);

                //draw toggle for each enum value
                enumValue = GUI.Toggle(buttonPosition, enumValue == (enumValue | value), enumNames[index], Style.buttonStyle)
                    ? enumValue | value
                    : enumValue & ~value;
                //adjust position for next button
                buttonPosition.x += buttonPosition.width + buttonsSpacing;
            }

            if (EditorGUI.EndChangeCheck())
            {
                //-1 value will specify when the whole mask is selected
                property.intValue = enumValue == enumSum ? -1 : enumValue;
            }
            EditorGUI.EndProperty();
        }


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            singleLineHeight = base.GetPropertyHeightSafe(property, label);
            //TODO:
            //NOTE: caching additional height like this will cause 1 frame delay 
            return singleLineHeight + additionalHeight;
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (((EnumFlagAttribute)attribute).Style)
            {
                case EnumStyle.Button:
                    DrawButtonsGui(position, property, label, declaringObject);
                    break;
                case EnumStyle.Popup:
                default:
                    DrawDefaultGui(position, property, label, declaringObject);
                    break;

            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                return false;
            }

            declaringObject = property.GetDeclaringObject();
            return Attribute.IsDefined(property.GetProperType(fieldInfo, declaringObject), typeof(FlagsAttribute));
        }


        private static class Style
        {
            internal static readonly GUIStyle buttonStyle;

            static Style()
            {
                buttonStyle = new GUIStyle(GUI.skin.button)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 10
#else
                    fontSize = 9
#endif
                };
            }
        }
    }
}