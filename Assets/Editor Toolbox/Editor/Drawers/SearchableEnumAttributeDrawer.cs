using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
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

            //TODO: 
            EditorGUI.PropertyField(position, property);
        }
    }
}