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
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //we want to cache all possible fields 
            const BindingFlags bindingFlags = BindingFlags.Instance  | BindingFlags.Static | 
                                              BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

            var targetObject = property.GetDeclaringObject();
            var presetValues = targetObject.GetType().GetField(Attribute.PresetFieldName, bindingFlags);
          
            if (presetValues == null)
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    "Cannot find relative preset field (" + Attribute.PresetFieldName + "). Property will be drawn in the standard way.");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var presetObject = presetValues.GetValue(targetObject);
            if (presetObject is IList)
            {
                var propertyType = property.GetProperType(fieldInfo, targetObject);
                //check if types match between property and provided preset
                if (propertyType == (presetValues.FieldType.IsGenericType 
                                   ? presetValues.FieldType.GetGenericArguments()[0] 
                                   : presetValues.FieldType.GetElementType()))
                {
                    var list = presetObject as IList;
                    var values = new object[list.Count];
                    var options = new string[list.Count];

                    for (var i = 0; i < list.Count; i++)
                    {
                        values[i] = list[i];
                        options[i] = list[i]?.ToString();
                    }

                    var index = Array.IndexOf(values, property.GetProperValue(fieldInfo, targetObject));

                    //begin the true property
                    label = EditorGUI.BeginProperty(position, label, property);
                    //draw the prefix label
                    position = EditorGUI.PrefixLabel(position, label);

                    EditorGUI.BeginChangeCheck();
                    //get index value from popup
                    index = EditorGUI.Popup(position, index, options);
                    //validate index value
                    index = Mathf.Clamp(index, 0, list.Count - 1);

                    if (EditorGUI.EndChangeCheck())
                    {
                        //udpate property value using previously cached FieldInfo and picked value
                        //there is no cleaner way to do it, since we don't really know what kind of 
                        //serialized property we are updating

                        property.serializedObject.Update(); 
                        property.SetProperValue(fieldInfo, values[index]);
                        property.serializedObject.ApplyModifiedProperties();

                        //handle situation when we update multiple different values
                        property.serializedObject.SetIsDifferentCacheDirty();
                    }
                    EditorGUI.EndProperty();
                }
                else
                {
                    ToolboxEditorLog.AttributeUsageWarning(attribute, property, 
                        "Type mismatch between serialized property and provided preset field. Property will be drawn in the standard way.");
                    EditorGUI.PropertyField(position, property, label);
                    return;
                }
            }
            else
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property, 
                    "Preset field (" + Attribute.PresetFieldName + ") has to be a one-dimensional collection(array or list). Property will be drawn in the standard way.");
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