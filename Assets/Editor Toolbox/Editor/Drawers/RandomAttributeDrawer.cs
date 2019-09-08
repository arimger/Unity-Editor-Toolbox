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

            //adjust field width to additional button
            var fieldRect = new Rect(position.x, position.y, position.width - Style.buttonWidth - Style.spacing, position.height);
            //set button rect aligned to field rect
            var buttonRect = new Rect(position.xMax - Style.buttonWidth, position.y, Style.buttonWidth, position.height);

            EditorGUI.PropertyField(fieldRect, property, property.isExpanded);
            //draw random button using custom style class
            if (GUI.Button(buttonRect, Style.buttonContent))
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
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="global::RandomAttribute"/>.
        /// </summary>
        private RandomAttribute RandomAttribute => attribute as RandomAttribute;


        /// <summary>
        /// Style representation.
        /// </summary>
        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float buttonWidth = 30.0f;

            internal static readonly GUIContent buttonContent;

            static Style()
            {
                buttonContent = new GUIContent(EditorGUIUtility.FindTexture("LookDevResetEnv@2x"), "Set random value");
            }
        }
    }
}