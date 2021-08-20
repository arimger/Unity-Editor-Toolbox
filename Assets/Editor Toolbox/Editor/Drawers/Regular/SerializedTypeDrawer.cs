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
        /// <summary>
        /// Dictionary used to store all previously filtered types matched to the targt attribute.
        /// </summary>
        private readonly static Dictionary<int, List<Type>> cachedfilteredTypes = new Dictionary<int, List<Type>>();


        /// <summary>
        /// Creates formatted type name depending on <see cref="ClassGrouping"/> value.
        /// </summary>
        /// <param name="type">Type to display.</param>
        /// <param name="grouping">Format grouping type.</param>
        private static string FormatGroupedTypeName(Type type, ClassGrouping grouping)
        {
            var name = type.FullName;
            switch (grouping)
            {
                default:
                case ClassGrouping.None:
                    return name;

                case ClassGrouping.ByNamespace:
                    return name.Replace('.', '/');

                case ClassGrouping.ByNamespaceFlat:
                    var lastPeriodIndex = name.LastIndexOf('.');
                    if (lastPeriodIndex != -1)
                    {
                        name = name.Substring(0, lastPeriodIndex) + "/" + name.Substring(lastPeriodIndex + 1);
                    }
                    return name;

                case ClassGrouping.ByAddComponentMenu:
                    var addComponentMenuAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
                    if (addComponentMenuAttributes.Length == 1)
                    {
                        return ((AddComponentMenu)addComponentMenuAttributes[0]).componentMenu;
                    }

                    return "Scripts/" + type.FullName.Replace('.', '/');
            }
        }

        /// <summary>
        /// Returns valid <see cref="string"/> equivalent of the referenced <see cref="Type"/>.
        /// </summary>
        private static string GetClassReferencValue(int selectedType, List<Type> types)
        {
            return selectedType > 0 ? SerializedType.GetClassReference(types[selectedType - 1]) : string.Empty;
        }

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
            return new ClassExtendsAttribute()
            {
                AddTextSearchField = true
            };
        }

        /// <summary>
        /// Returns all <see cref="Type"/>s associated to the given constraint.
        /// </summary>
        private List<Type> GetFilteredTypes(TypeConstraintAttribute attribute)
        {
            var hashCode = attribute.GetHashCode();
            if (cachedfilteredTypes.TryGetValue(hashCode, out var filteredTypes))
            {
                return filteredTypes;
            }
            else
            {
                return cachedfilteredTypes[hashCode] = attribute.GetFilteredTypes();
            }
        }


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.CalcHeight(GUIContent.none, 0);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var validAttribute = GetVerifiedAttribute(attribute);

            var referenceProperty = property.FindPropertyRelative("classReference");
            var referenceValue = referenceProperty.stringValue;
            var currentType = !string.IsNullOrEmpty(referenceValue) ? Type.GetType(referenceValue) : null;

            var filteredTypes = GetFilteredTypes(validAttribute);

            var itemsCount = filteredTypes.Count + 1;
            var options = new string[itemsCount];
            var index = 0;

            //create labels for all types
            options[0] = "<None>";
            for (var i = 1; i < itemsCount; i++)
            {
                var menuType = filteredTypes[i - 1];
                var menuLabel = FormatGroupedTypeName(menuType, validAttribute.Grouping);
                if (menuType == currentType)
                {
                    index = i;
                }

                options[i] = menuLabel;
            }

            //draw the reference property
            label = EditorGUI.BeginProperty(position, label, property);
            label = property.name != "data" ? label : GUIContent.none;
            //draw the proper label field
            position = EditorGUI.PrefixLabel(position, label);

            //try to draw associated popup
            if (validAttribute.AddTextSearchField)
            {
                var buttonLabel = new GUIContent(options[index]);
                ToolboxEditorGui.DrawSearchablePopup(position, buttonLabel, index, options, (i) =>
                {
                    try
                    {
                        referenceProperty.serializedObject.Update();
                        referenceProperty.stringValue = GetClassReferencValue(i, filteredTypes);
                        referenceProperty.serializedObject.ApplyModifiedProperties();
                    }
                    catch (Exception e) when (e is ArgumentNullException || e is NullReferenceException)
                    {
                        ToolboxEditorLog.LogWarning("Invalid attempt to update disposed property.");
                    }
                });
            }
            else
            {
                using (new ZeroIndentScope())
                {
                    EditorGUI.BeginChangeCheck();
                    index = EditorGUI.Popup(position, index, options);
                    if (EditorGUI.EndChangeCheck())
                    {
                        referenceProperty.stringValue = GetClassReferencValue(index, filteredTypes);
                    }
                }
            }

            EditorGUI.EndProperty();
        }


        ///<inheritdoc/>
        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.type == nameof(SerializedType);
        }
    }
}