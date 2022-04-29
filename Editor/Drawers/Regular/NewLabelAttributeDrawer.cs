using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [Obsolete("For now, NewLabelAttribute is handled internally by the ToolboxPropertyHandler.")]
    public class NewLabelAttributeDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var newLabel = new GUIContent(Attribute.NewLabel);
            var oldLabel = new GUIContent(Attribute.OldLabel ?? label.text);

            //NOTE: we have to perform BeginProperty to prevent label overriding
            label = EditorGUI.BeginProperty(position, label, property);
            label.text = property.displayName.Replace(oldLabel.text, newLabel.text);
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
            EditorGUI.EndProperty();
        }


        private NewLabelAttribute Attribute => attribute as NewLabelAttribute;
    }
}