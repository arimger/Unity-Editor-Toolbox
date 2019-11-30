using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHideAttributeDrawer : ConditionalAttributeDrawer
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