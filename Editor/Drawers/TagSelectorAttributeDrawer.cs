using System.Collections.Generic;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : ToolboxNativePropertyDrawer
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //begin the true property
            label = EditorGUI.BeginProperty(position, label, property);
            //draw the prefix label
            position = EditorGUI.PrefixLabel(position, label);

            var tags = new List<string>
            {
                "<None>"
            };
            tags.AddRange(InternalEditorUtility.tags);
            var value = property.stringValue;
            var index = -1;

            if (value == "")
            {
                index = 0;
            }
            else
            {
                for (int i = 1; i < tags.Count; i++)
                {
                    if (tags[i] == value)
                    {
                        index = i;
                        break;
                    }
                }
            }

            //draw the popup window
            index = EditorGUI.Popup(position, index, tags.ToArray());
            //cache last picked value
            property.stringValue = index >= 1 ? tags[index] : "";
            //end all property controls
            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }


        private TagSelectorAttribute Attribute => attribute as TagSelectorAttribute;
    }
}