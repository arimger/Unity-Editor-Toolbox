using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [Obsolete("For now, HideLabelAttribute is handled internally by the ToolboxPropertyHandler.")]
    public class HideLabelAttributeDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, GUIContent.none, property.isExpanded);
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return true;
        }
    }
}