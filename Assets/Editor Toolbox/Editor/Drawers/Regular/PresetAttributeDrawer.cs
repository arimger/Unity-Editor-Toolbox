using System.Collections;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PresetAttribute))]
    public class PresetAttributeDrawer : PropertyDrawerBase
    {
        private IList GetPresetList(object parentObject, string sourceHandle, SerializedProperty property)
        {
            if (!ValueExtractionHelper.TryGetValue(sourceHandle, parentObject, out var sourceValue))
            {
                ToolboxEditorLog.MemberNotFoundWarning(attribute, property, sourceHandle);
                return null;
            }

            if (!(sourceValue is IList presetList))
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    string.Format("Values preset ({0}) has to be a one-dimensional collection (array or list).", sourceHandle));
                return null;
            }

            var sourceType = sourceValue.GetType();
            var targetType = property.GetProperType(fieldInfo);
            //check if types match between property and provided preset
            if (targetType != (sourceType.IsGenericType
                             ? sourceType.GetGenericArguments()[0]
                             : sourceType.GetElementType()))
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    "Type mismatch between serialized property and given Preset.");
                return null;
            }

            return presetList;
        }

        private string[] GetOptions(IList presetList)
        {
            var itemsCount = presetList.Count;
            var options = new string[itemsCount];
            for (var i = 0; i < itemsCount; i++)
            {
                options[i] = presetList[i]?.ToString();
            }

            return options;
        }

        private string[] GetOptions(IList presetList, object parentObject, string optionHandle, SerializedProperty property)
        {
            if (string.IsNullOrEmpty(optionHandle))
            {
                return GetOptions(presetList);
            }

            if (!ValueExtractionHelper.TryGetValue(optionHandle, parentObject, out var optionValue))
            {
                ToolboxEditorLog.MemberNotFoundWarning(attribute, property, optionHandle);
                return GetOptions(presetList);
            }

            if (!(optionValue is IList optionList))
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                       string.Format("Options preset ({0}) has to be a one-dimensional collection (array or list).", optionHandle));
                return GetOptions(presetList);
            }

            var presetsCount = presetList.Count;
            var optionsCount = optionList.Count;
            var options = new string[presetsCount];
            for (int i = 0; i < presetsCount; i++)
            {
                options[i] = i < optionsCount
                    ? optionList[i]?.ToString()
                    : presetList[i]?.ToString();
            }

            return options;
        }


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //NOTE: this implementation does not support multiple different sources
            var sourceHandle = Attribute.SourceHandle;
            var optionHandle = Attribute.OptionHandle;
            var parentObject = property.GetDeclaringObject();
            var presetList = GetPresetList(parentObject, sourceHandle, property);
            if (presetList == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var options = GetOptions(presetList, parentObject, optionHandle, property);

            var value = property.GetProperValue(fieldInfo, parentObject);
            var index = presetList.IndexOf(value);

            //begin the true property
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            //get selected preset value
            index = EditorGUI.Popup(position, label, index, EditorGUIUtility.TrTempContent(options));
            index = Mathf.Clamp(index, 0, presetList.Count - 1);
            if (EditorGUI.EndChangeCheck())
            {
                //udpate property value using previously cached FieldInfo and picked value
                //there is no cleaner way to do it, since we don't really know what kind of 
                //serialized property we are updating

                property.serializedObject.Update();
                property.SetProperValue(fieldInfo, presetList[index]);
                property.serializedObject.ApplyModifiedProperties();
                //handle situation when updating multiple different targets
                property.serializedObject.SetIsDifferentCacheDirty();
            }

            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            var parentObject = property.GetDeclaringObject();
            //NOTE: reflection won't work properly on nested structs because of boxing
            //TODO: handle this case
            return !parentObject.GetType().IsValueType;
        }


        private PresetAttribute Attribute => attribute as PresetAttribute;
    }
}