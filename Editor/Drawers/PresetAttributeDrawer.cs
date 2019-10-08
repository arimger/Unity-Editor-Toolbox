using System;
using System.Collections;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PresetAttribute))]
    public class PresetAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var presetTarget = property.serializedObject.targetObject;
            var presetTargets = property.serializedObject.targetObjects;
            var presetValues = ReflectionUtility.GetField(presetTarget, Attribute.PresetPropertyName);
            if (presetValues == null)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - Cannot find relative preset field " + Attribute.PresetPropertyName + ". Property will be drawn in standard way.");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var presetObject = presetValues.GetValue(presetTarget);
            if (presetObject is IList)
            {
                //check if types match between property and provided preset
                if (fieldInfo.FieldType == (presetValues.FieldType.IsGenericType ? presetValues.FieldType.GetGenericArguments()[0] : presetValues.FieldType.GetElementType()))
                {
                    var list = presetObject as IList;
                    var values = new object[list.Count];
                    var options = new string[list.Count];
                    for (var i = 0; i < list.Count; i++)
                    {
                        values[i] = list[i];
                        options[i] = list[i]?.ToString();
                    }

                    var index = Array.IndexOf(values, fieldInfo.GetValue(presetTarget));

                    EditorGUI.BeginProperty(position, label, property);
                    EditorGUI.BeginChangeCheck();
                    //get index value from popup
                    index = EditorGUI.Popup(position, label.text, index, options);
                    //validate index value
                    index = Mathf.Clamp(index, 0, list.Count - 1);
                    if (EditorGUI.EndChangeCheck())
                    {
                        //udpate property value using fieldInfo field
                        property.serializedObject.Update();
                        foreach (var target in presetTargets)
                        {
                            fieldInfo.SetValue(target, values[index]);

                            //workaround to call OnValidate during fieldInfo edit
                            if (target is MonoBehaviour)
                            {
                                (target as MonoBehaviour).SendMessage("OnValidate", SendMessageOptions.DontRequireReceiver);
                            }
                        }

                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.SetIsDifferentCacheDirty();
                    }
                    EditorGUI.EndProperty();
                }
                else
                {
                    Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                     " - type mismatch between serialized property and provided preset field. Property will be drawn in standard way.");
                    EditorGUI.PropertyField(position, property, label);
                    return;
                }
            }
            else
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - preset field(" + Attribute.PresetPropertyName + ") has to be a one-dimensional collection(array or list). Property will be drawn in standard way.");
                EditorGUI.PropertyField(position, property, label);
                return;
            }
        }


        private PresetAttribute Attribute => attribute as PresetAttribute;
    }
}