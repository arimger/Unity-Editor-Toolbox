using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel += Attribute.IndentLevelChange;
            EditorGUI.PropertyField(position, property, property.isExpanded);
            EditorGUI.indentLevel -= Attribute.IndentLevelChange;
        }

        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="global::IndentAttribute"/>.
        /// </summary>
        private IndentAttribute Attribute => attribute as IndentAttribute;
    }
}