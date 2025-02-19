#if UNITY_2019_3_OR_NEWER
using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;
    using Toolbox.Editor.Internal.Types;

    public class ReferencePickerAttributeDrawer : ToolboxSelfPropertyDrawer<ReferencePickerAttribute>
    {
        private const float labelWidthOffset = -80.0f;

        private static readonly TypeConstraintContext sharedConstraint = new TypeConstraintSerializeReference(null);
        private static readonly TypeAppearanceContext sharedAppearance = new TypeAppearanceContext(sharedConstraint, TypeGrouping.None, true);
        private static readonly TypeField typeField = new TypeField(sharedConstraint, sharedAppearance);

        private void UpdateContexts(ReferencePickerAttribute attribute)
        {
            sharedAppearance.TypeGrouping = attribute.TypeGrouping;
        }

        private Type GetParentType(ReferencePickerAttribute attribute, SerializedProperty property)
        {
            var fieldInfo = property.GetFieldInfo(out _);
            var fieldType = property.GetProperType(fieldInfo);
            var candidateType = attribute.ParentType;
            if (candidateType != null)
            {
                if (fieldType.IsAssignableFrom(candidateType))
                {
                    return candidateType;
                }

                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    $"Provided {nameof(attribute.ParentType)} ({candidateType}) cannot be used because it's not assignable from: '{fieldType}'");
            }

            return fieldType;
        }

        private static Type GetCurrentManagedReferenceType(SerializedProperty property, out bool hasMixedValues)
        {
            var fullTypeName = property.managedReferenceFullTypename;
            hasMixedValues = false;
            TypeUtility.TryGetTypeFromManagedReferenceFullTypeName(fullTypeName, out var targetType);

            var currentSerializedObject = property.serializedObject;
            if (currentSerializedObject.isEditingMultipleObjects)
            {
                var targets = currentSerializedObject.targetObjects;
                foreach (var target in targets)
                {
                    using (var tempSerializedObject = new SerializedObject(target))
                    {
                        var tempProperty = tempSerializedObject.FindProperty(property.propertyPath);
                        if (tempProperty.managedReferenceFullTypename != fullTypeName)
                        {
                            hasMixedValues = true;
                            break;
                        }
                    }
                }
            }

            return targetType;
        }

        private void CreateTypeProperty(SerializedProperty property, Type parentType, ReferencePickerAttribute attribute, Rect position)
        {
            var currentType = GetCurrentManagedReferenceType(property, out var hasMixedValues);
            var hadMixedValues = EditorGUI.showMixedValue;
            EditorGUI.showMixedValue = hasMixedValues;
            typeField.OnGui(position, attribute.AddTextSearchField, (type) =>
            {
                try
                {
                    if (!property.serializedObject.isEditingMultipleObjects)
                    {
                        UpdateTypeProperty(property, type, attribute);
                    }
                    else
                    {
                        var targets = property.serializedObject.targetObjects;
                        foreach (var target in targets)
                        {
                            using (var so = new SerializedObject(target))
                            {
                                var sp = so.FindProperty(property.propertyPath);
                                UpdateTypeProperty(sp, type, attribute);
                            }
                        }
                    }
                }
                catch (Exception e) when (e is ArgumentNullException || e is NullReferenceException)
                {
                    ToolboxEditorLog.LogWarning("Invalid attempt to update disposed property.");
                }
            }, currentType, parentType);
            EditorGUI.showMixedValue = false;
        }

        private void UpdateTypeProperty(SerializedProperty property, Type targetType, ReferencePickerAttribute attribute)
        {
            var forceUninitializedInstance = attribute.ForceUninitializedInstance;
            var obj = ReflectionUtility.CreateInstance(targetType, forceUninitializedInstance);
            property.serializedObject.Update();
            property.managedReferenceValue = obj;
            property.serializedObject.ApplyModifiedProperties();

            //NOTE: fix for invalid cached properties, e.g. changing parent's managed reference can change available children
            // since we cannot check if cached property is "valid" we need to clear the whole cache
            //TODO: reverse it and provide dedicated event when a managed property is changed through a dedicated handler
            DrawerStorageManager.ClearStorages();
        }

        private Rect PrepareTypePropertyPosition(bool hasLabel, in Rect labelPosition, in Rect inputPosition, bool isPropertyExpanded)
        {
            var position = new Rect(inputPosition);
            if (!hasLabel)
            {
                position.xMin += EditorGUIUtility.standardVerticalSpacing;
                return position;
            }

            //skip row only if label exists
            if (isPropertyExpanded)
            {
                //property is expanded and we have place to move it to the next row
                position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                position = EditorGUI.IndentedRect(position);
                return position;
            }

            var baseLabelWidth = EditorGUIUtility.labelWidth + labelWidthOffset;
            var realLabelWidth = labelPosition.width;
            //adjust position to already rendered label
            position.xMin += Mathf.Max(baseLabelWidth, realLabelWidth);
            return position;
        }

        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ReferencePickerAttribute attribute)
        {
            //NOTE: we want to close scope manually because ExitGUIException can interrupt drawing and SerializedProperties stack
            using (var propertyScope = new PropertyScope(property, label, closeManually: true))
            {
                UpdateContexts(attribute);

                var isPropertyExpanded = propertyScope.IsVisible;
                EditorGUI.indentLevel++;
                var labelRect = propertyScope.LabelRect;
                var inputRect = propertyScope.InputRect;

                var hasLabel = !string.IsNullOrEmpty(label.text);
                var position = PrepareTypePropertyPosition(hasLabel, in labelRect, in inputRect, isPropertyExpanded);

                var parentType = GetParentType(attribute, property);
                CreateTypeProperty(property, parentType, attribute, position);
                if (isPropertyExpanded)
                {
                    ToolboxEditorGui.DrawPropertyChildren(property);
                }

                EditorGUI.indentLevel--;
                propertyScope.Close();
            }
        }

        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ManagedReference;
        }
    }
}
#endif