using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHideAttributeDrawer : ConditionalAttributeDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsConditionMet(property) ? EditorGUI.GetPropertyHeight(property) : -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsConditionMet(property)) EditorGUI.PropertyField(position, property, label, true);
        }
    }
}