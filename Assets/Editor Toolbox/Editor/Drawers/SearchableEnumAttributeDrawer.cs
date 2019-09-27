using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public class SearchableEnumAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on enum value properties.");
                EditorGUI.PropertyField(position, property);
                return;
            }

            var buttonLabel = property.enumValueIndex >= 0 && property.enumValueIndex < property.enumDisplayNames.Length
                ? new GUIContent(property.enumDisplayNames[property.enumValueIndex])
                : new GUIContent();
            var id = GUIUtility.GetControlID(FocusType.Keyboard, position);
            //draw prefix label and begin true property
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);
            //draw dropdown button, will be used to activate popup
            if (EditorGUI.DropdownButton(position, buttonLabel, FocusType.Keyboard))
            {
                SearchablePopup.Show(position, property.enumValueIndex, property.enumDisplayNames, (i) =>
                {
                    property.serializedObject.Update();
                    property.enumValueIndex = i;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            EditorGUI.EndProperty();
        }
    }
}