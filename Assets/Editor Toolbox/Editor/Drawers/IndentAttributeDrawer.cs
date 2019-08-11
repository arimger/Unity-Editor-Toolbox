using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel += IndentAttribute.IndentLevelChange;
            EditorGUI.PropertyField(position, property, property.isExpanded);
            EditorGUI.indentLevel -= IndentAttribute.IndentLevelChange;
        }

        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="global::IndentAttribute"/>.
        /// </summary>
        private IndentAttribute IndentAttribute => attribute as IndentAttribute;
    }
}