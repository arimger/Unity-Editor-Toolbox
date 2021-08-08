using System;
using System.Collections;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Reflection;

    [CustomPropertyDrawer(typeof(PresetAttribute))]
    public class PresetAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var sourceName = Attribute.SourceName;
            var targetObject = property.GetDeclaringObject();
            if (!ValueExtractionHelper.TryGetValue(sourceName, targetObject, out var presetSource))
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    string.Format("Cannot find relative preset source ({0}).", sourceName));
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (presetSource is IList list)
            {
                var presetSourceType = presetSource.GetType();
                var propertyType = property.GetProperType(fieldInfo, targetObject);
                //check if types match between property and provided preset
                if (propertyType == (presetSourceType.IsGenericType
                                   ? presetSourceType.GetGenericArguments()[0]
                                   : presetSourceType.GetElementType()))
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
                    string.Format("Preset source ({0}) has to be a one-dimensional collection (array or list).", sourceName));
                EditorGUI.PropertyField(position, property, label);
                return;
            }
        }

        public override bool IsPropertyValid(SerializedProperty property)
        {
            //NOTE: reflection won't work properly on nested structs because of boxing
            return !property.GetDeclaringObject().GetType().IsValueType;
        }


        private PresetAttribute Attribute => attribute as PresetAttribute;
    }
}