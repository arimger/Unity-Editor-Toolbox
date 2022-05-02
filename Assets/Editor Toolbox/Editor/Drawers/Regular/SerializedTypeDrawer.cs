using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    [CustomPropertyDrawer(typeof(TypeConstraintAttribute), true)]
    [CustomPropertyDrawer(typeof(SerializedType))]
    public sealed class SerializedTypeDrawer : PropertyDrawerBase
    {
        private static readonly TypeField typeField = new TypeField(true, true, new TypeConstraintStandard());


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


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.CalcHeight(GUIContent.none, 0);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var validAttribute = GetVerifiedAttribute(attribute);

            //TODO: update constraints
            typeField.TypeGrouping = validAttribute.TypeGrouping;
            typeField.TypeConstraint = new TypeConstraintStandard(validAttribute.AssemblyType,
                validAttribute.TypeSettings, validAttribute.AllowAbstract, validAttribute.AllowObsolete);
  
            var referenceProperty = property.FindPropertyRelative("classReference");
            var referenceValue = referenceProperty.stringValue;
            var activeType = !string.IsNullOrEmpty(referenceValue) ? Type.GetType(referenceValue) : null;

            label = EditorGUI.BeginProperty(position, label, property);
            label = property.name != "data" ? label : GUIContent.none;
            position = EditorGUI.PrefixLabel(position, label);
            typeField.OnGui(position, validAttribute.AssemblyType, activeType, (type) =>
            {
                referenceProperty.serializedObject.Update();
                referenceProperty.stringValue = SerializedType.GetClassReference(type);
                referenceProperty.serializedObject.ApplyModifiedProperties();
            });

            EditorGUI.EndProperty();
        }


        ///<inheritdoc/>
        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.type == nameof(SerializedType);
        }
    }
}