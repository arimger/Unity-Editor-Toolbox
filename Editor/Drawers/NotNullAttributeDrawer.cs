using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(NotNullAttribute))]
    public class NotNullAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue)
            {
                return base.GetPropertyHeightSafe(property, label);
            }

            //set additional height as help box height + 2x spacing between properties
            return base.GetPropertyHeightSafe(property, label) + Style.boxHeight + Style.spacing * 2;
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //set up proper height for this property field
            position.height = EditorGUI.GetPropertyHeight(property);

            var errorColor = GUI.backgroundColor;
            var cacheColor = GUI.backgroundColor;

            if (property.objectReferenceValue == null)
            {
                errorColor = Style.backgroundColor;

                var helpBoxRect = new Rect(position.x, 
                                           position.y, 
                                           position.width, Style.boxHeight);
                EditorGUI.HelpBox(helpBoxRect, Attribute.Label, MessageType.Error);
                position.y += Style.boxHeight + Style.spacing * 2;
            }

            //finally draw the property
            GUI.backgroundColor = errorColor;
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
            GUI.backgroundColor = cacheColor;
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }


        private NotNullAttribute Attribute => attribute as NotNullAttribute;


        private static class Style
        {
            internal static readonly float rowHeight = EditorGUIUtility.singleLineHeight;
#if UNITY_2019_3_OR_NEWER
            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 2.1f;
#else
            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 2.5f;
#endif
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float padding = 5.0f;

            internal static readonly Color backgroundColor = Color.red;
        }
    }
}