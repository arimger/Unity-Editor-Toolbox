using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(NewLabelAttribute))]
    public class NewLabelAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var newLabel = new GUIContent(NewLabelAttribute.NewLabel);
            var oldLabel = new GUIContent(NewLabelAttribute.OldLabel ?? label.text);
            label.text = property.displayName.Replace(oldLabel.text, newLabel.text);
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }

        private NewLabelAttribute NewLabelAttribute => attribute as NewLabelAttribute;
    }
}