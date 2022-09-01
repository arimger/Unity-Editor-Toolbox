using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    [CustomPropertyDrawer(typeof(TypeConstraintAttribute), true)]
    [CustomPropertyDrawer(typeof(SerializedType))]
    public sealed class SerializedTypeDrawer : PropertyDrawerBase
    {
        private static readonly TypeConstraintContext sharedConstraint = new TypeConstraintStandard();
        private static readonly TypeAppearanceContext sharedAppearance = new TypeAppearanceContext(sharedConstraint, TypeGrouping.None, true);
        private static readonly TypeField typeField = new TypeField(sharedConstraint, sharedAppearance);


        private bool IsDefaultField(TypeConstraintAttribute attribute)
        {
            return attribute == null || attribute.AssemblyType == null;
        }

        /// <summary>
        /// Creates default constraint attribute if the given one is invalid.
        /// </summary>
        private TypeConstraintAttribute GetVerifiedAttribute(Attribute attribute)
        {
            return GetVerifiedAttribute(attribute as TypeConstraintAttribute);
        }

        /// <summary>
        /// Creates default constraint attribute if the given one is invalid.
        /// </summary>
        private TypeConstraintAttribute GetVerifiedAttribute(TypeConstraintAttribute attribute)
        {
            return IsDefaultField(attribute) ? GetDefaultConstraint() : attribute;
        }

        /// <summary>
        /// Returns default <see cref="TypeConstraintAttribute"/>.
        /// </summary>
        private TypeConstraintAttribute GetDefaultConstraint()
        {
            return new ClassExtendsAttribute(typeof(object))
            {
                AddTextSearchField = true
            };
        }

        private void UpdateConstraint(TypeConstraintAttribute attribute)
        {
            sharedConstraint.ApplyTarget(attribute.AssemblyType);
            if (sharedConstraint is TypeConstraintStandard constraint)
            {
                constraint.IsOrdered = attribute.OrderTypes;
                constraint.AllowAbstract = attribute.AllowAbstract;
                constraint.AllowObsolete = attribute.AllowObsolete;
                constraint.Settings = attribute.TypeSettings;
            }
        }

        private void UpdateAppearance(TypeConstraintAttribute attribute)
        {
            sharedAppearance.TypeGrouping = attribute.TypeGrouping;
        }


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.CalcHeight(GUIContent.none, 0);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            label = property.name != "data" ? label : GUIContent.none;
            position = EditorGUI.PrefixLabel(position, label);

            var validAttribute = GetVerifiedAttribute(attribute);
            var addSearchField = validAttribute.AddTextSearchField;
            UpdateConstraint(validAttribute);
            UpdateAppearance(validAttribute);

            var referenceProperty = property.FindPropertyRelative("typeReference");
            var activeType = SerializedType.GetReferenceType(referenceProperty.stringValue);
            typeField.OnGui(position, addSearchField, (type) =>
            {
                try
                {
                    referenceProperty.serializedObject.Update();
                    referenceProperty.stringValue = SerializedType.GetReferenceValue(type);
                    referenceProperty.serializedObject.ApplyModifiedProperties();
                }
                catch (Exception e) when (e is ArgumentNullException || e is NullReferenceException)
                {
                    ToolboxEditorLog.LogWarning("Invalid attempt to update disposed property.");
                }
            }, activeType);

            EditorGUI.EndProperty();
        }


        ///<inheritdoc/>
        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.type == nameof(SerializedType);
        }
    }
}