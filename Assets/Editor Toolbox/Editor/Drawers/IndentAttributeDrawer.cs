using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel += Attribute.IndentLevelChange;
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
            EditorGUI.indentLevel -= Attribute.IndentLevelChange;
        }


        private IndentAttribute Attribute => attribute as IndentAttribute;
    }
}