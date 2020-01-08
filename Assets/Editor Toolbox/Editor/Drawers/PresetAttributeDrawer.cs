using System;
using System.Collections;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PresetAttribute))]
    public class PresetAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetObject = property.GetDeclaringObject();
            var presetValues = targetObject.GetType().GetField(Attribute.PresetPropertyName, ReflectionUtility.allPossibleFieldsBinding);
          
            if (presetValues == null)
            {
                LogWarning(property, attribute, "Cannot find relative preset field " + Attribute.PresetPropertyName + ".Property will be drawn in standard way.");
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

                    EditorGUI.BeginProperty(position, label, property);
                    EditorGUI.BeginChangeCheck();
                    //get index value from popup
                    index = EditorGUI.Popup(position, label.text, index, options);
                    //validate index value
                    index = Mathf.Clamp(index, 0, list.Count - 1);

                    if (EditorGUI.EndChangeCheck())
                    {
                        //udpate property value
                        property.serializedObject.Update(); 
                        property.SetProperValue(fieldInfo, values[index]);
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.SetIsDifferentCacheDirty();
                    }
                    EditorGUI.EndProperty();
                }
                else
                {
                    LogWarning(property, attribute, "Type mismatch between serialized property and provided preset field. Property will be drawn in standard way.");
                    EditorGUI.PropertyField(position, property, label);
                    return;
                }
            }
            else
            {
                LogWarning(property, attribute, "Preset field (" + Attribute.PresetPropertyName + ") has to be a one-dimensional collection(array or list). Property will be drawn in standard way.");
                EditorGUI.PropertyField(position, property, label);
                return;
            }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }


        private PresetAttribute Attribute => attribute as PresetAttribute;
    }
}