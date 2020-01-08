using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel += Attribute.IndentLevelChange;
            EditorGUI.PropertyField(position, property, property.isExpanded);
            EditorGUI.indentLevel -= Attribute.IndentLevelChange;
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="IndentAttribute"/>.
        /// </summary>
        private IndentAttribute Attribute => attribute as IndentAttribute;
    }
}