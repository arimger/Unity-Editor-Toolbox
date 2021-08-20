using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public class SearchableEnumAttributeDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //prepare pick button label
            var buttonLabel = property.enumValueIndex >= 0 &&
                              property.enumValueIndex < property.enumDisplayNames.Length
                ? new GUIContent(property.enumDisplayNames[property.enumValueIndex])
                : new GUIContent();
            var id = GUIUtility.GetControlID(FocusType.Keyboard, position);

            //begin the true property
            label = EditorGUI.BeginProperty(position, label, property);
            //draw the prefix label
            position = EditorGUI.PrefixLabel(position, id, label);
            //draw dropdown button, will be used to activate popup
            ToolboxEditorGui.DrawSearchablePopup(position, buttonLabel, property.enumValueIndex, property.enumDisplayNames, (i) =>
            {
                property.serializedObject.Update();
                property.enumValueIndex = i;
                property.serializedObject.ApplyModifiedProperties();
            });
            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Enum;
        }
    }
}