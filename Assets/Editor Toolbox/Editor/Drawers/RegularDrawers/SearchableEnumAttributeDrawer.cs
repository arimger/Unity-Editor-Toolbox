using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public class SearchableEnumAttributeDrawer : ToolboxNativePropertyDrawer
    {
        private static EditorWindow lastSearchableWindow;


        /// <summary>
        /// Checks if user is still focusing the proper (searchable) window.
        /// </summary>
        private bool IsNonTargetWindowFocused()
        {
            return lastSearchableWindow && lastSearchableWindow != EditorWindow.mouseOverWindow;
        }


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
            if (EditorGUI.DropdownButton(position, buttonLabel, FocusType.Keyboard))
            {
                try
                {
                    SearchablePopup.Show(position, property.enumValueIndex, property.enumDisplayNames, (i) =>
                    {
                        property.serializedObject.Update();
                        property.enumValueIndex = i;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                catch (ExitGUIException)
                {
                    lastSearchableWindow = EditorWindow.focusedWindow;
                    throw;
                }
            }
            EditorGUI.EndProperty();

            //handle situation when inspector window captures ScrollWheel event
            if (IsNonTargetWindowFocused())
            {
                //NOTE: unfortunately PopupWidnows are not indpendent and we have to
                //override internal events to prevent scrolling the Inspector Window
                if (Event.current.type == EventType.ScrollWheel)
                {
                    Event.current.Use();
                }
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Enum;
        }
    }
}