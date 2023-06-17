using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(LabelWidthAttribute))]
    public class LabelWidthAttributeDrawer : PropertyDrawerBase
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var previousWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Attribute.Width;
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
            EditorGUIUtility.labelWidth = previousWidth;
        }

        private LabelWidthAttribute Attribute => attribute as LabelWidthAttribute;
    }
}