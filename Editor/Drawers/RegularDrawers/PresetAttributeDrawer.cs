using System;
using System.Collections;
using System.Reflection;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PresetAttribute))]
    public class PresetAttributeDrawer : ToolboxNativePropertyDrawer
    {
        //TODO: move it to the ReflectionUtility
        private const BindingFlags presetBinding = BindingFlags.Instance | BindingFlags.Static |
                                                   BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetObject = property.GetDeclaringObject();
            var presetValues = targetObject.GetType().GetField(Attribute.PresetFieldName, presetBinding);  
            if (presetValues == null)
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    "Cannot find relative preset field (" + Attribute.PresetFieldName + ").");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var presetObject = presetValues.GetValue(targetObject);
            if (presetObject is IList list)
            {
                var propertyType = property.GetProperType(fieldInfo, targetObject);
                //check if types match between property and provided preset
                if (propertyType == (presetValues.FieldType.IsGenericType 
                                   ? presetValues.FieldType.GetGenericArguments()[0] 
                                   : presetValues.FieldType.GetElementType()))
                {
                    var objects = new object[list.Count];
                    var options = new string[list.Count];

                    for (var i = 0; i < list.Count; i++)
                    {
                        objects[i] = list[i];
                        options[i] = list[i]?.ToString();
                    }

                    var index = Array.IndexOf(objects, property.GetProperValue(fieldInfo, targetObject));

                    //begin the true property
                    label = EditorGUI.BeginProperty(position, label, property);
                    //draw the prefix label
                    position = EditorGUI.PrefixLabel(position, label);

                    EditorGUI.BeginChangeCheck();
                    //get selected preset value
                    index = EditorGUI.Popup(position, index, options);
                    //validate index before set
                    index = Mathf.Clamp(index, 0, list.Count - 1);
                    if (EditorGUI.EndChangeCheck())
                    {
                        //udpate property value using previously cached FieldInfo and picked value
                        //there is no cleaner way to do it, since we don't really know what kind of 
                        //serialized property we are updating

                        property.serializedObject.Update(); 
                        property.SetProperValue(fieldInfo, objects[index]);
                        property.serializedObject.ApplyModifiedProperties();

                        //handle situation when updating multiple different targets
                        property.serializedObject.SetIsDifferentCacheDirty();
                    }

                    EditorGUI.EndProperty();
                }
                else
                {
                    ToolboxEditorLog.AttributeUsageWarning(attribute, property, 
                        "Type mismatch between serialized property and provided preset field.");
                    EditorGUI.PropertyField(position, property, label);
                    return;
                }
            }
            else
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property, 
                    "Preset field (" + Attribute.PresetFieldName + ") has to be a one-dimensional collection (array or list).");
                EditorGUI.PropertyField(position, property, label);
                return;
            }
        }

        public override bool IsPropertyValid(SerializedProperty property)
        {
            //NOTE: reflection won't work properly on nested structs
            return !property.GetDeclaringObject().GetType().IsValueType;
        }


        private PresetAttribute Attribute => attribute as PresetAttribute;
    }
}