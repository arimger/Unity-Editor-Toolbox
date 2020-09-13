using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [Obsolete]
    [CustomPropertyDrawer(typeof(ConditionalEnableAttribute))]
    public class ConditionalEnableAttributeDrawer : ConditionalAttributeDrawer
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var disable = !IsConditionMet(property);
            EditorGUI.BeginDisabledGroup(disable);
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
            EditorGUI.EndDisabledGroup();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return true;
        }
    }
}