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
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
            EditorGUI.indentLevel -= Attribute.IndentLevelChange;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }


        private IndentAttribute Attribute => attribute as IndentAttribute;
    }
}