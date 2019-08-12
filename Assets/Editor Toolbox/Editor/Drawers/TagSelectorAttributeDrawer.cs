using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on string properties.");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            if (TagSelectorAttribute.UseDefaultTagFieldDrawer)
            {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            }
            else
            {
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
            }

            EditorGUI.EndProperty();
        }


        private TagSelectorAttribute TagSelectorAttribute => attribute as TagSelectorAttribute;
    }
}