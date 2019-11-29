using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : ToolboxNativeDrawerBase
    {
        protected override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            EditorGUI.BeginProperty(position, label, property);
            var tags = new List<string>
            {
                "<None>"
            };
            tags.AddRange(InternalEditorUtility.tags);
            var propertyString = property.stringValue;
            var index = -1;
            if (propertyString == "")
            {
                index = 0;
            }
            else
            {
                for (int i = 1; i < tags.Count; i++)
                {
                    if (tags[i] == propertyString)
                    {
                        index = i;
                        break;
                    }
                }
            }

            index = EditorGUI.Popup(position, label.text, index, tags.ToArray());

            property.stringValue = index >= 1 ? tags[index] : "";

            EditorGUI.EndProperty();
        }


        private TagSelectorAttribute Attribute => attribute as TagSelectorAttribute;
    }
}