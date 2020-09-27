using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SuffixAttribute))]
    public class SuffixAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyLabel = new GUIContent(property.displayName);
            //set up needed fields
            var suffixLabel = new GUIContent(Attribute.SuffixLabel);
            var suffixStyle = new GUIStyle(EditorStyles.miniLabel);
            var suffixSize = suffixStyle.CalcSize(suffixLabel);
            var suffixRect = new Rect(position.xMax - suffixSize.x, position.y, suffixSize.x, position.height);

            position.xMax -= suffixSize.x;

            //draw suffix label
            EditorGUI.LabelField(suffixRect, suffixLabel, suffixStyle);
            //draw standard property field
            EditorGUI.PropertyField(position, property, propertyLabel, property.isExpanded);
        }


        private SuffixAttribute Attribute => attribute as SuffixAttribute;
    }
}