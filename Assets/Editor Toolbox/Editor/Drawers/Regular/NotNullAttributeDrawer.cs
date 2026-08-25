using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    [CustomPropertyDrawer(typeof(NotNullAttribute))]
    public class NotNullAttributeDrawer : PropertyDrawerBase
    {
        private static Color GetBackgroundColor(MessageType type)
        {
            switch (type)
            {
                case MessageType.Info:
                    return Style.infoBackgroundColor;
                case MessageType.Warning:
                    return Style.warningBackgroundColor;
                default:
                    return Style.errorBackgroundColor;
            }
        }

        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return property.objectReferenceValue
                ? base.GetPropertyHeightSafe(property, label)
                : base.GetPropertyHeightSafe(property, label) + Style.boxHeight + Style.spacing * 2;
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUI.GetPropertyHeight(property);

            if (property.objectReferenceValue)
            {
                //draw property in the default way
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
            }
            else
            {
                var messageType = (MessageType)Attribute.MessageType;

                //create respective HelpBox information
                var helpBoxRect = new Rect(position.x,
                                           position.y,
                                           position.width, Style.boxHeight);
                EditorGUI.HelpBox(helpBoxRect, Attribute.Label, messageType);
                position.y += Style.boxHeight + Style.spacing * 2;

                //change temporary GUI background color 
                var backgroundColor = GetBackgroundColor(messageType);
                using (new GuiBackground(backgroundColor))
                {
                    EditorGUI.PropertyField(position, property, label, property.isExpanded);
                }
            }
        }

        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }

        private NotNullAttribute Attribute => attribute as NotNullAttribute;

        private static class Style
        {
#if UNITY_2019_3_OR_NEWER
            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 2.1f;
#else
            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 2.5f;
#endif
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;

            internal static readonly Color infoBackgroundColor = Color.white;
            internal static readonly Color errorBackgroundColor = Color.red;
            internal static readonly Color warningBackgroundColor = Color.yellow;
        }
    }
}
