using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(NewLabelAttribute))]
    public class NewLabelAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var newLabel = new GUIContent(Attribute.NewLabel);
            var oldLabel = new GUIContent(Attribute.OldLabel ?? label.text);
            label.text = property.displayName.Replace(oldLabel.text, newLabel.text);
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }


        private NewLabelAttribute Attribute => attribute as NewLabelAttribute;
    }
}