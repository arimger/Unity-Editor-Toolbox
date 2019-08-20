using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(SerializedTypeReference))]
    [CustomPropertyDrawer(typeof(ClassTypeConstraintAttribute), true)]
    public sealed class SerializedTypeReferenceDrawer : PropertyDrawer
    {
        #region Type Filtering

        private static List<Type> GetFilteredTypes(ClassTypeConstraintAttribute filter)
        {
            var types = new List<Type>();
            var assembly = Assembly.GetExecutingAssembly();

            types.AddRange(GetFilteredAssemblyTypes(assembly, filter));

            foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
            {
                types.AddRange(GetFilteredAssemblyTypes(Assembly.Load(referencedAssembly), filter));
            }

            types.Sort((a, b) => a.FullName.CompareTo(b.FullName));

            return types;
        }

        private static List<Type> GetFilteredAssemblyTypes(Assembly assembly, ClassTypeConstraintAttribute filter)
        {
            var types = new List<Type>();
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsVisible || !type.IsClass)
                    continue;

                if (filter != null && !filter.IsConstraintSatisfied(type))
                    continue;

                types.Add(type);
            }

            return types;
        }

        #endregion


        private static int selectionControlID;

        [Obsolete]
        private static string selectedClassRef;


        private static Type ResolveType(string classRef)
        {
            return !string.IsNullOrEmpty(classRef) ? Type.GetType(classRef) : null;
        }

        /// <summary>
        /// Creates formatted type name depending on <see cref="ClassGrouping"/> value.
        /// </summary>
        /// <param name="type">Type to display.</param>
        /// <param name="grouping">Format grouping type.</param>
        /// <returns></returns>
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

        [Obsolete]
        private void DrawTypeSelectionRect(Rect position, SerializedProperty property, ClassTypeConstraintAttribute filter, GUIContent label)
        {
            if (label != null && label != GUIContent.none)
            {
                position = EditorGUI.PrefixLabel(position, label);
            }

            var evnt = Event.current;
            var classRef = property.stringValue;
            var controlID = GUIUtility.GetControlID(typeof(SerializedTypeReferenceDrawer).GetHashCode(), FocusType.Keyboard, position);
            var triggerDropDown = false;

            switch (evnt.GetTypeForControl(controlID))
            {
                case EventType.ExecuteCommand:

                    if (evnt.commandName == "TypeReferenceUpdated")
                    {
                        if (selectionControlID == controlID)
                        {
                            if (classRef != selectedClassRef)
                            {
                                classRef = selectedClassRef;
                                GUI.changed = true;
                            }

                            selectionControlID = 0;
                            selectedClassRef = null;
                        }
                    }
                    break;

                case EventType.MouseDown:

                    if (GUI.enabled && position.Contains(evnt.mousePosition))
                    {
                        GUIUtility.keyboardControl = controlID;
                        triggerDropDown = true;
                        evnt.Use();
                    }
                    break;

                case EventType.KeyDown:

                    if (GUI.enabled && GUIUtility.keyboardControl == controlID)
                    {
                        if (evnt.keyCode == KeyCode.Return || evnt.keyCode == KeyCode.Space)
                        {
                            triggerDropDown = true;
                            evnt.Use();
                        }
                    }
                    break;

                case EventType.Repaint:

                    var classRefParts = classRef.Split(',');
                    var content = new GUIContent(classRefParts[0].Trim());

                    if (property.hasMultipleDifferentValues)
                    {
                        content.text = "[Multiple]";
                    }
                    else if (content.text == "")
                    {
                        content.text = "<None>";
                    }
                    else if (ResolveType(classRef) == null)
                    {
                        content.text += " {Missing}";
                    }

                    EditorStyles.popup.Draw(position, content, controlID);
                    break;
            }

            if (triggerDropDown)
            {
                selectedClassRef = classRef;
                selectionControlID = controlID;
                DrawTypeDropDown(position, property, filter);
            }

            property.stringValue = classRef;
        }
        [Obsolete]
        private void DrawTypeDropDown(Rect rect, SerializedProperty property, ClassTypeConstraintAttribute filter)
        {
            var refType = ResolveType(property.stringValue);
            var refTypes = GetFilteredTypes(filter);
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("<None>"), refType == null, OnTypeSelected, null);
            menu.AddSeparator("");

            for (int i = 0; i < refTypes.Count; i++)
            {
                var type = refTypes[i];
                var menuLabel = FormatGroupedTypeName(type, filter.Grouping);

                if (string.IsNullOrEmpty(menuLabel)) continue;

                menu.AddItem(new GUIContent(menuLabel), refType == type, OnTypeSelected, type);
            }

            menu.DropDown(rect);
        }
        [Obsolete]
        private void OnTypeSelected(object userData)
        {
            selectedClassRef = SerializedTypeReference.GetClassReference(userData as Type);
            EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("TypeReferenceUpdated"));
        }


        private readonly static OrderedDictionary filteredTypes = new OrderedDictionary();


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.CalcHeight(GUIContent.none, 0);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var refAttribute = ClassTypeConstraintAttribute;
            var refProperty = property.FindPropertyRelative("classReference");

            if (refProperty == null)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on string properties.");
                EditorGUI.PropertyField(rect, property, label);
                return;
            }

            if (refAttribute == null || refAttribute.AssemblyType == null)
            {
                EditorGUI.PropertyField(rect, refProperty, label);
                return;
            }

            //DrawTypeSelectionRect(position, refProperty, refAttribute, label);

            var refType = ResolveType(refProperty.stringValue);
            var refTypes = new List<Type>();
            var refLabels = new List<string>() { "<None>" };
            var index = -1;

            //getting all filtered and associated types to provided attribute
            if (filteredTypes.Contains(refAttribute.AssemblyType))
            {
                refTypes = filteredTypes[refAttribute.AssemblyType] as List<Type>;
            }
            //getting all needed types and saving results to dictionary
            else
            {
                refTypes = GetFilteredTypes(refAttribute);
                filteredTypes.Add(refAttribute.AssemblyType, refTypes);
            }

            //creating labels from filtered types
            for (int i = 0; i < refTypes.Count; i++)
            {
                var menuType = refTypes[i];
                var menuLabel = FormatGroupedTypeName(menuType, refAttribute.Grouping);

                if (menuType == refType) index = i;

                refLabels.Add(menuLabel);
            }

            //drawing reference property
            EditorGUI.BeginProperty(rect, label, refProperty);
            label = property.name != "data" ? label : GUIContent.none;
            index = EditorGUI.Popup(rect, label.text, index + 1, refLabels.ToArray());
            //getting correct class reference, index = 0 is reserved to <None> type
            refProperty.stringValue = index >= 1 ? SerializedTypeReference.GetClassReference(refTypes[index - 1]) : "";
            EditorGUI.EndProperty();        
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="global::ClassTypeConstraintAttribute"/>.
        /// </summary>
        private ClassTypeConstraintAttribute ClassTypeConstraintAttribute => attribute as ClassTypeConstraintAttribute;
    }
}