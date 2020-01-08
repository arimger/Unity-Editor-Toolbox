using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(NotNullAttribute))]
    public class NotNullAttributeDrawer : ToolboxNativePropertyDrawer
    {
        /// <summary>
        /// Creates optional HelpBox if reference value is null and draws standard property label.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //set up proper height for this property field
            position.height = EditorGUI.GetPropertyHeight(property);

            var warnColor = GUI.backgroundColor;
            var cachedColor = GUI.backgroundColor;

            if (property.objectReferenceValue == null)
            {
                var helpBoxRect = new Rect(position.x, position.y, position.width, Style.boxHeight);

                EditorGUI.HelpBox(helpBoxRect, Attribute.Label, MessageType.Error);

                //set additional height as help box height + 2x spacing between properties
                position.y += Style.boxHeight + Style.spacing * 2;
                //set proper warning color 
                warnColor = Style.backgroundColor;
            }

            //finally draw property
            GUI.backgroundColor = warnColor;
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
            GUI.backgroundColor = cachedColor;
        }


        /// <summary>
        /// Checks if provided property has valid type.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }

        /// <summary>
        /// Overall property height, depending on aditional height.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!IsPropertyValid(property) || property.objectReferenceValue)
            {
                return base.GetPropertyHeight(property, label);
            }

            //set additional height as help box height + 2x spacing between properties
            return base.GetPropertyHeight(property, label) + Style.boxHeight + Style.spacing * 2;
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="NotNullAttribute"/>.
        /// </summary>
        private NotNullAttribute Attribute => attribute as NotNullAttribute;


        /// <summary>
        /// Custom style representation.
        /// </summary>
        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 1.25f * 2;
            internal static readonly float padding = 5.0f;

            internal static readonly Color backgroundColor = Color.red;
        }
    }
}