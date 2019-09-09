using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(NewLabelAttribute))]
    public class NewLabelAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var newLabel = new GUIContent(Attribute.NewLabel);
            var oldLabel = new GUIContent(Attribute.OldLabel ?? label.text);
            label.text = property.displayName.Replace(oldLabel.text, newLabel.text);
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }


        private NewLabelAttribute Attribute => attribute as NewLabelAttribute;
    }
}