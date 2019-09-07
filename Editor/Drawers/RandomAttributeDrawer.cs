using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(RandomAttribute))]
    public class RandomAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on float or int value properties.");
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
                return;
            }

            if (GUI.Button(new Rect(position.x, position.y, Style.buttonWidth, position.height), Style.buttonContent))
            {
                var random = Random.Range(RandomAttribute.MinValue, RandomAttribute.MaxValue);
                if (property.propertyType == SerializedPropertyType.Integer)
                {
                    property.intValue = (int)random;
                }
                else
                {
                    property.floatValue = random;
                }
            }

            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, property, property.isExpanded);
            EditorGUI.indentLevel--;
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="global::RandomAttribute"/>.
        /// </summary>
        private RandomAttribute RandomAttribute => attribute as RandomAttribute;


        private static class Style
        {
            internal static readonly float buttonWidth = 15.0f;

            internal static readonly GUIContent buttonContent;

            static Style()
            {
                buttonContent = new GUIContent("?", "Set random value");
            }
        }
    }
}