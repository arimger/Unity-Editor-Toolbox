using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(RandomAttribute))]
    public class RandomAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //adjust field width to additional button
            var fieldRect = new Rect(position.x, position.y, position.width - Style.buttonWidth - Style.spacing, position.height);
            //set button rect aligned to field rect
            var buttonRect = new Rect(position.xMax - Style.buttonWidth, position.y, Style.buttonWidth, position.height);

            EditorGUI.PropertyField(fieldRect, property, property.isExpanded);
            //draw random button using custom style class
            if (GUI.Button(buttonRect, Style.buttonContent))
            {
                var random = Random.Range(Attribute.MinValue, Attribute.MaxValue);
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    property.floatValue = random;
                }
                else
                {
                    property.intValue = (int)random;
                }
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Float ||
                   property.propertyType == SerializedPropertyType.Integer;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="RandomAttribute"/>.
        /// </summary>
        private RandomAttribute Attribute => attribute as RandomAttribute;


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