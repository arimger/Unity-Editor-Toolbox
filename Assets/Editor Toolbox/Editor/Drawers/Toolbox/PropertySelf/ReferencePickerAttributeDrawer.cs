#if UNITY_2019_3_OR_NEWER
using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ReferencePickerAttributeDrawer : ToolboxSelfPropertyDrawer<ReferencePickerAttribute>
    {
        private const float labelWidthOffset = -80.0f;

        private static readonly TypeConstraintContext sharedConstraint = new TypeConstraintReference(null);
        private static readonly TypeAppearanceContext sharedAppearance = new TypeAppearanceContext(sharedConstraint, TypeGrouping.None, true);
        private static readonly TypeField typeField = new TypeField(sharedConstraint, sharedAppearance);


        private void UpdateContexts(ReferencePickerAttribute attribute)
        {
            sharedAppearance.TypeGrouping = attribute.TypeGrouping;
        }

        private Type GetParentType(SerializedProperty property, ReferencePickerAttribute attribute)
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

        private void CreateTypeProperty(Rect position, SerializedProperty property, Type parentType)
        {
            TypeUtilities.TryGetTypeFromManagedReferenceFullTypeName(property.managedReferenceFullTypename, out var currentType);
            typeField.OnGui(position, true, (type) =>
            {
                try
                {
                    if (!property.serializedObject.isEditingMultipleObjects)
                    {
                        UpdateTypeProperty(property, type);
                    }
                    else
                    {
                        var targets = property.serializedObject.targetObjects;
                        foreach (var target in targets)
                        {
                            using (var so = new SerializedObject(target))
                            {
                                SerializedProperty sp = so.FindProperty(property.propertyPath);
                                UpdateTypeProperty(sp, type);
                            }
                        }
                    }
                }
                catch (Exception e) when (e is ArgumentNullException || e is NullReferenceException)
                {
                    ToolboxEditorLog.LogWarning("Invalid attempt to update disposed property.");
                }
            }, currentType, parentType);
        }

        private void UpdateTypeProperty(SerializedProperty property, Type referenceType)
        {
            var obj = referenceType != null ? Activator.CreateInstance(referenceType) : null;
            property.serializedObject.Update();
            property.managedReferenceValue = obj;
            property.serializedObject.ApplyModifiedProperties();
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

                var parentType = GetParentType(property, attribute);
                CreateTypeProperty(position, property, parentType);
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