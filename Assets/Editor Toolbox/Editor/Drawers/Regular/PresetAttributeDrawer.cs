using System;
using System.Collections;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PresetAttribute))]
    public class PresetAttributeDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //NOTE: this implementation does not support multiple different sources
            var sourceName = Attribute.SourceHandle;
            var declaringObject = property.GetDeclaringObject();
            //extract (if available) the real preset value
            if (!ValueExtractionHelper.TryGetValue(sourceName, declaringObject, out var sourceValue))
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    string.Format("Cannot find relative Preset ({0}).", sourceName));
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (!(sourceValue is IList presetList))
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    string.Format("Preset ({0}) has to be a one-dimensional collection (array or list).", sourceName));
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var sourceType = sourceValue.GetType();
            var targetType = property.GetProperType(fieldInfo, declaringObject);
            //check if types match between property and provided preset
            if (targetType != (sourceType.IsGenericType
                             ? sourceType.GetGenericArguments()[0]
                             : sourceType.GetElementType()))
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    "Type mismatch between serialized property and given Preset.");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var itemsCount = presetList.Count;
            var objects = new object[itemsCount];
            var options = new string[itemsCount];
            for (var i = 0; i < itemsCount; i++)
            {
                objects[i] = presetList[i];
                options[i] = presetList[i]?.ToString();
            }

            var value = property.GetProperValue(fieldInfo, declaringObject);
            var index = Array.IndexOf(objects, value);

            //begin the true property
            label = EditorGUI.BeginProperty(position, label, property);
            //draw the prefix label
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();
            //get selected preset value
            index = EditorGUI.Popup(position, index, options);
            index = Mathf.Clamp(index, 0, itemsCount - 1);
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


        public override bool IsPropertyValid(SerializedProperty property)
        {
            var declaringObject = property.GetDeclaringObject();
            //NOTE: reflection won't work properly on nested structs because of boxing
            return !declaringObject.GetType().IsValueType;
        }


        private PresetAttribute Attribute => attribute as PresetAttribute;
    }
}