using System;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SerializedDateTime))]
    public class SerializedDateTimeDrawer : PropertyDrawerBase
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            var fieldPosition = EditorGUI.PrefixLabel(position, label);
            var ticksProperty = property.FindPropertyRelative("ticks");
            DateTime dateTime = new DateTime(ticksProperty.longValue);
            EditorGUI.BeginChangeCheck();
            var dateTimeString = EditorGUI.DelayedTextField(fieldPosition, dateTime.ToString());
            if (EditorGUI.EndChangeCheck())
            {
                if (DateTime.TryParse(dateTimeString, out var newDateTime))
                {
                    ticksProperty.serializedObject.Update();
                    ticksProperty.longValue = newDateTime.Ticks;
                    ticksProperty.serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUI.EndProperty();
        }
    }
}