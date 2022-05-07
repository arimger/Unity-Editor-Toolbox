#if UNITY_2019_3_OR_NEWER
using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ReferencePickerAttributeDrawer : ToolboxSelfPropertyDrawer<ReferencePickerAttribute>
    {
        private readonly TypeField typeField = new TypeField(new TypeConstraintReference(null));


        private void CreateTypeProperty(SerializedProperty property)
        {
            property.GetFieldInfo(out Type propertyType);
            TypeUtilities.TryGetTypeFromManagedReferenceFullTypeName(property.managedReferenceFullTypename, out var currentType);
            var position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
            position = EditorGUI.IndentedRect(position);
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
            }, currentType, propertyType);
        }

        private void UpdateTypeProperty(SerializedProperty property, Type referenceType)
        {
            var obj = referenceType != null ? Activator.CreateInstance(referenceType) : null;
            property.serializedObject.Update();
            property.managedReferenceValue = obj;
            property.serializedObject.ApplyModifiedProperties();
        }


        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ReferencePickerAttribute attribute)
        {
            using (var propertyScope = new PropertyScope(property, label))
            {
                if (!propertyScope.IsVisible)
                {
                    return;
                }

                EditorGUI.indentLevel++;
                CreateTypeProperty(property);
                ToolboxEditorGui.DrawPropertyChildren(property);
                EditorGUI.indentLevel--;
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ManagedReference;
        }
    }
}
#endif