using System;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [Obsolete]
    [CustomPropertyDrawer(typeof(ConditionalShowAttribute))]
    public class ConditionalShowAttributeDrawer : ConditionalAttributeDrawer
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsConditionMet(property)) EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsConditionMet(property) ? EditorGUI.GetPropertyHeight(property) : -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}