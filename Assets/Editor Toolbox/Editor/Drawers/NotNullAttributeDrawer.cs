using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(NotNullAttribute))]
    public class NotNullAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Needed height for additional HelpBox.
        /// </summary>
        private float additionalHeight;


        /// <summary>
        /// Overall property height, depending on aditional height.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + additionalHeight;
        }

        /// <summary>
        /// Creates optional HelpBox if needed(if reference value is null) and draws standard property label.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on reference value properties.");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            additionalHeight = 0;

            var warnColor = GUI.backgroundColor;
            var cachedColor = GUI.backgroundColor;
      
            if (property.objectReferenceValue == null)
            {
                var helpBoxRect = new Rect(position.x, position.y, position.width, Style.height);

                EditorGUI.HelpBox(helpBoxRect, Attribute.Label, MessageType.Error);

                //set additional height as help box height + 2x spacing between properties
                additionalHeight = Style.height + Style.spacing * 2;
                position.height -= additionalHeight;
                //adjust OY position for target property
                position.y += additionalHeight;

                warnColor = Style.backgroundColor;
            }

            GUI.backgroundColor = warnColor;
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
            GUI.backgroundColor = cachedColor;
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="NotNullAttribute"/>.
        /// </summary>
        private NotNullAttribute Attribute => attribute as NotNullAttribute;


        /// <summary>
        /// Static representation of box style.
        /// </summary>
        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight * 1.25f * 2;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float padding = 5.0f;

            internal static readonly Color backgroundColor = Color.red;
        }
    }
}