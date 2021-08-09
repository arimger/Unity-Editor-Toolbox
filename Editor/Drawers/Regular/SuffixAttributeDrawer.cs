using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SuffixAttribute))]
    public class SuffixAttributeDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //set up needed fields
            var suffixLabel = new GUIContent(Attribute.SuffixLabel);
            var suffixStyle = new GUIStyle(EditorStyles.miniLabel);
            var suffixWidth = suffixStyle.CalcSize(suffixLabel).x;

            position.xMax -= suffixWidth;
            //draw standard property field
            EditorGUI.PropertyField(position, property, label, property.isExpanded);

            position.xMin = position.xMax;
            position.xMax += suffixWidth;
            //draw suffix label
            EditorGUI.LabelField(position, suffixLabel, suffixStyle);
        }


        private SuffixAttribute Attribute => attribute as SuffixAttribute;
    }
}