using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public class SearchableEnumAttributeDrawer : ToolboxNativePropertyDrawer
    {
        private static EditorWindow lastSearchableWindow;


        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //prepare pick button label
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
            if (lastSearchableWindow && lastSearchableWindow != EditorWindow.mouseOverWindow)
            {
                //NOTE: unfortunately PopupWidnows are not indpendent and we have to
                //handle this case inside drawer since this is the only way to override window events
                if (Event.current.type == EventType.ScrollWheel) Event.current.Use();
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Enum;
        }
    }
}