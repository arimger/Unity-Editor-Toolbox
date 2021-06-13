using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ClassTypeConstraintAttribute), true)]
    [CustomPropertyDrawer(typeof(SerializedType))]
    public sealed class SerializedTypeDrawer : ToolboxNativePropertyDrawer
    {
        /// <summary>
        /// Dictionary used to store all previously filtered types.
        /// </summary>
        private readonly static Dictionary<Type, List<Type>> filteredTypes = new Dictionary<Type, List<Type>>();


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

        private bool IsDefaultField(Attribute attribute)
        {
            return IsDefaultField(attribute as ClassTypeConstraintAttribute);
        }

        private bool IsDefaultField(ClassTypeConstraintAttribute attribute)
        {
            return attribute == null || attribute.AssemblyType == null;
        }


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return IsDefaultField(attribute)
                ? EditorGUI.GetPropertyHeight(property)
                : EditorStyles.popup.CalcHeight(GUIContent.none, 0);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = Attribute;
            //TODO: default drawer for fields without attributes
            //validate serialized field
            if (IsDefaultField(attribute))
            {
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
                return;
            }

            //TODO: cache different filter settings
            //get stored types if possible or try to cache them
            if (!filteredTypes.TryGetValue(attribute.AssemblyType, out var refTypes))
            {
                filteredTypes[attribute.AssemblyType] = refTypes = attribute.GetFilteredTypes();
            }

            var referenceProperty = property.FindPropertyRelative("classReference");
            var referenceValue = referenceProperty.stringValue;
            var referenceType = !string.IsNullOrEmpty(referenceValue) ? Type.GetType(referenceValue) : null;
            var optionsCount = refTypes.Count + 1;
            var options = new string[optionsCount];
            var index = 0;

            //create labels for all types
            options[0] = "<None>";
            for (var i = 1; i < optionsCount; i++)
            {
                var menuType = refTypes[i - 1];
                var menuLabel = FormatGroupedTypeName(menuType, attribute.Grouping);
                if (menuType == referenceType)
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
            if (attribute.AddTextSearchField)
            {
                var buttonLabel = new GUIContent(options[index]);
                ToolboxEditorGui.DrawSearchablePopup(position, buttonLabel, index, options, (i) =>
                {
                    referenceProperty.serializedObject.Update();
                    referenceProperty.stringValue = GetClassReferencValue(i, refTypes);
                    referenceProperty.serializedObject.ApplyModifiedProperties();
                });
            }
            else
            {
                index = EditorGUI.Popup(position, index, options);
                referenceProperty.stringValue = GetClassReferencValue(index, refTypes);
            }

            EditorGUI.EndProperty();
        }


        ///<inheritdoc/>
        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.type == nameof(SerializedType);
        }


        private ClassTypeConstraintAttribute Attribute => attribute as ClassTypeConstraintAttribute;
    }
}