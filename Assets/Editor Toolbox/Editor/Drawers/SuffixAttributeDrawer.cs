using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SuffixAttribute))]
    public class SuffixAttributeDrawer : ToolboxNativePropertyDrawer
    {
        /// <summary>
        /// Draws validated property.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
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


        /// <summary>
        /// Returns current height needed by this drawer.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="SuffixAttribute"/>.
        /// </summary>
        private SuffixAttribute Attribute => attribute as SuffixAttribute;
    }
}