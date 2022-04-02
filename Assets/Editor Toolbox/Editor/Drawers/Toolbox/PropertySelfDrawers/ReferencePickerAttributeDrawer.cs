using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ReferencePickerAttributeDrawer : ToolboxSelfPropertyDrawer<ReferencePickerAttribute>
    {
        private readonly static Dictionary<int, List<Type>> cachedfilteredTypes = new Dictionary<int, List<Type>>();

        //TODO: additional utility class, something like "EditorTypesUtility"
        private static bool TryGetTypeFromManagedReferenceFullTypeName(string managedReferenceFullTypename, out Type managedReferenceInstanceType)
        {
            managedReferenceInstanceType = null;

            var parts = managedReferenceFullTypename.Split(' ');
            if (parts.Length == 2)
            {
                var assemblyPart = parts[0];
                var nsClassnamePart = parts[1];
                //TODO: cache it
                managedReferenceInstanceType = Type.GetType($"{nsClassnamePart}, {assemblyPart}");
            }

            return managedReferenceInstanceType != null;
        }

        private static List<Type> GetFilteredTypes(Type type)
        {
            var hashCode = type.GetHashCode();
            if (cachedfilteredTypes.TryGetValue(hashCode, out var filteredTypes))
            {
                return filteredTypes;
            }
            else
            {
                return cachedfilteredTypes[hashCode] = TypeCache.GetTypesDerivedFrom(type).ToList();
            }
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
                //TODO: handle null
                TryGetTypeFromManagedReferenceFullTypeName(property.managedReferenceFullTypename, out var currentType);
                var fieldInfo = property.GetFieldInfo(out Type mainType);
                var filteredTypes = GetFilteredTypes(mainType);

                var itemsCount = filteredTypes.Count;
                var options = new string[itemsCount];
                var index = 0;

                //create labels for all types
                for (var i = 0; i < itemsCount; i++)
                {
                    var menuType = filteredTypes[i];
                    var menuLabel = menuType.FullName;
                    if (menuType == currentType)
                    {
                        index = i;
                    }

                    options[i] = menuLabel;
                }

                var buttonLabel = new GUIContent(options[index]);
                //TODO: move it to ToolboxEditorGui
                var position = EditorGUILayout.GetControlRect(false, 16.0f);
                ToolboxEditorGui.DrawSearchablePopup(position, buttonLabel, index, options, (i) =>
                {
                    try
                    {
                        //TODO: handle multiple objects
                        property.serializedObject.Update();
                        var type = i >= 0 ? filteredTypes[i] : null;
                        var obj = type != null ? Activator.CreateInstance(type) : null;
                        property.managedReferenceValue = obj;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    catch (Exception e) when (e is ArgumentNullException || e is NullReferenceException)
                    {
                        ToolboxEditorLog.LogWarning("Invalid attempt to update disposed property.");
                    }
                });

                ToolboxEditorGui.DrawPropertyChildren(property);
                EditorGUI.indentLevel--;
            }
        }

        public override bool IsPropertyValid(SerializedProperty property)
        {
            //TODO: validate property (check for the SerializeReferenceAttribute)
            return true;
        }
    }
}