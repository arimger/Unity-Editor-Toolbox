using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(SuffixAttribute))]
    public class SuffixAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyLabel = new GUIContent(property.displayName);
            //set up needed fields
            var suffixLabel = new GUIContent(SuffixAttribute.SuffixLabel);
            var suffixStyle = new GUIStyle(EditorStyles.miniLabel);
            var suffixSize = suffixStyle.CalcSize(suffixLabel);
            var suffixRect = new Rect(position.xMax - suffixSize.x, position.y, suffixSize.x, position.height);

            position.xMax -= suffixSize.x;

            //draw suffix label
            EditorGUI.LabelField(suffixRect, suffixLabel, suffixStyle);
            //draw standard property field
            EditorGUI.PropertyField(position, property, propertyLabel, property.isExpanded);
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="global::SuffixAttribute"/>.
        /// </summary>
        private SuffixAttribute SuffixAttribute => attribute as SuffixAttribute;
    }
}