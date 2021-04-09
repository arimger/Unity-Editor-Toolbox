using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    internal static class ToolboxDrawerModule
    {
        [InitializeOnLoadMethod]
        internal static void InitializeModule()
        {
            InspectorUtility.OnEditorReload += ReloadDrawers;
        }


        private readonly static Type decoratorDrawerBase = typeof(ToolboxDecoratorDrawer<>);
        private readonly static Type conditionDrawerBase = typeof(ToolboxConditionDrawer<>);
        private readonly static Type selfPropertyDrawerBase = typeof(ToolboxSelfPropertyDrawer<>);
        private readonly static Type listPropertyDrawerBase = typeof(ToolboxListPropertyDrawer<>);

        private readonly static Dictionary<Type, ToolboxDecoratorDrawerBase> decoratorDrawers = new Dictionary<Type, ToolboxDecoratorDrawerBase>();
        private readonly static Dictionary<Type, ToolboxConditionDrawerBase> conditionDrawers = new Dictionary<Type, ToolboxConditionDrawerBase>();
        private readonly static Dictionary<Type, ToolboxPropertyDrawerBase> selfPropertyDrawers = new Dictionary<Type, ToolboxPropertyDrawerBase>();
        private readonly static Dictionary<Type, ToolboxPropertyDrawerBase> listPropertyDrawers = new Dictionary<Type, ToolboxPropertyDrawerBase>();

        /// <summary>
        /// Collection of specific drawers mapped to selected (picked) object types.
        /// </summary>
        private readonly static Dictionary<Type, ToolboxTargetTypeDrawer> targetTypeDrawers = new Dictionary<Type, ToolboxTargetTypeDrawer>();

        /// <summary>
        /// Collection of currently cached handlers mapped to a unique property key.
        /// </summary>
        private static readonly Dictionary<string, ToolboxPropertyHandler> propertyHandlers = new Dictionary<string, ToolboxPropertyHandler>();


        /// <summary>
        /// Settings provided to handle custom drawers.
        /// </summary>
        private static IToolboxInspectorSettings settings;

        /// <summary>
        /// Determines if any invalid settings should be logged into the Console Window.
        /// </summary>
        private static bool validationEnabled = true;


        /// <summary>
        /// Creates all possible attribute-based drawers and add them to proper collections.
        /// </summary>
        private static void PrepareAssignableDrawers(IToolboxInspectorSettings settings)
        {
            void AddAttributeDrawer<T>(Type drawerType, Type attributeType, Dictionary<Type, T> drawersCollection) where T : ToolboxAttributeDrawer
            {
                if (drawerType == null)
                {
                    return;
                }

                var drawerInstance = Activator.CreateInstance(drawerType) as T;
                if (drawersCollection.ContainsKey(attributeType))
                {
                    if (validationEnabled)
                    {
                        ToolboxEditorLog.LogError("Attribute:" + attributeType + " is associated to more than one ToolboxDrawer.");
                    }

                    return;
                }

                drawersCollection.Add(attributeType, drawerInstance);
            }

            Type GetDrawersGenericType(Type drawerType, Type drawerBaseType)
            {
                if (drawerType == null)
                {
                    if (validationEnabled)
                    {
                        ToolboxEditorLog.LogWarning("One of assigned drawer types in the " + nameof(ToolboxEditorSettings) + " is empty.");
                    }

                    return null;
                }

                while (!drawerType.IsGenericType || drawerType.GetGenericTypeDefinition() != drawerBaseType)
                {
                    if (drawerType.BaseType == null)
                    {
                        return null;
                    }

                    drawerType = drawerType.BaseType;
                }

                return drawerType.IsGenericType ? drawerType.GetGenericArguments().FirstOrDefault() : null;
            }

            decoratorDrawers.Clear();
            for (var i = 0; i < settings.DecoratorDrawerHandlers.Count; i++)
            {
                var drawerType = settings.DecoratorDrawerHandlers[i].Type;
                var targetType = GetDrawersGenericType(drawerType, decoratorDrawerBase);
                AddAttributeDrawer(drawerType, targetType, decoratorDrawers);
            }

            conditionDrawers.Clear();
            for (var i = 0; i < settings.ConditionDrawerHandlers.Count; i++)
            {
                var drawerType = settings.ConditionDrawerHandlers[i].Type;
                var targetType = GetDrawersGenericType(drawerType, conditionDrawerBase);
                AddAttributeDrawer(drawerType, targetType, conditionDrawers);
            }

            selfPropertyDrawers.Clear();
            for (var i = 0; i < settings.SelfPropertyDrawerHandlers.Count; i++)
            {
                var drawerType = settings.SelfPropertyDrawerHandlers[i].Type;
                var targetType = GetDrawersGenericType(drawerType, selfPropertyDrawerBase);
                AddAttributeDrawer(drawerType, targetType, selfPropertyDrawers);
            }

            listPropertyDrawers.Clear();
            for (var i = 0; i < settings.ListPropertyDrawerHandlers.Count; i++)
            {
                var drawerType = settings.ListPropertyDrawerHandlers[i].Type;
                var targetType = GetDrawersGenericType(drawerType, listPropertyDrawerBase);
                AddAttributeDrawer(drawerType, targetType, listPropertyDrawers);
            }
        }

        /// <summary>
        /// Creates all possible type-based drawers and add them to the related collection.
        /// </summary>
        private static void PrepareTargetTypeDrawers(IToolboxInspectorSettings settings)
        {
            var childrenTypesMap = new Dictionary<ToolboxTargetTypeDrawer, List<Type>>();

            targetTypeDrawers.Clear();
            for (var i = 0; i < settings.TargetTypeDrawerHandlers.Count; i++)
            {
                var drawerType = settings.TargetTypeDrawerHandlers[i].Type;
                if (drawerType == null)
                {
                    if (validationEnabled)
                    {
                        ToolboxEditorLog.LogWarning("One of assigned drawer types in the " + nameof(ToolboxEditorSettings) + " is empty.");
                    }

                    continue;
                }

                var drawerInstance = Activator.CreateInstance(drawerType) as ToolboxTargetTypeDrawer;
                var targetBaseType = drawerInstance.GetTargetType();
                if (targetBaseType != null)
                {
                    if (targetTypeDrawers.ContainsKey(targetBaseType))
                    {
                        if (validationEnabled)
                        {
                            ToolboxEditorLog.LogError("Type:" + targetBaseType + " is associated to more than one ToolboxDrawer.");
                        }

                        continue;
                    }

                    targetTypeDrawers[targetBaseType] = drawerInstance;

                    if (drawerInstance.UseForChildren())
                    {
                        childrenTypesMap[drawerInstance] = targetBaseType.GetAllChildClasses();
                    }
                }
            }

            foreach (var typesMap in childrenTypesMap)
            {
                var typesDrawer = typesMap.Key;
                var targetTypes = typesMap.Value;

                for (var i = 0; i < targetTypes.Count; i++)
                {
                    var targetType = targetTypes[i];
                    if (targetTypeDrawers.ContainsKey(targetType))
                    {
                        continue;
                    }

                    targetTypeDrawers[targetType] = typesDrawer;
                }
            }
        }


        /// <summary>
        /// Clears all currently cached <see cref="ToolboxPropertyHandler"/>s.
        /// </summary>
        internal static void ReloadDrawers()
        {
            propertyHandlers.Clear();

            foreach (var drawer in listPropertyDrawers.Values)
            {
                drawer.OnGuiReload();
            }

            foreach (var drawer in selfPropertyDrawers.Values)
            {
                drawer.OnGuiReload();
            }
        }

        /// <summary>
        /// Initializes all possible drawers.
        /// </summary>
        internal static void UpdateDrawers()
        {
            UpdateDrawers(settings);
        }

        /// <summary>
        /// Initializes all assigned drawers using the given settings reference.
        /// </summary>
        internal static void UpdateDrawers(IToolboxInspectorSettings settings)
        {
            ToolboxDrawerModule.settings = settings;

            if (settings == null)
            {
                ToolboxDrawersAllowed = false;
                return;
            }

            ToolboxDrawersAllowed = settings.UseToolboxDrawers;

            //create all attribute-related drawers
            PrepareAssignableDrawers(settings);
            //create all type-only-related drawers
            PrepareTargetTypeDrawers(settings);

            //log errors into console only once
            validationEnabled = false;
        }


        /// <summary>
        /// Determines if property has any associated drawer (built-in or custom one).
        /// This method does not take into account <see cref="ToolboxDrawer"/>s.
        /// </summary>
        internal static bool HasNativeTypeDrawer(Type type)
        {
            var parameters = new object[] { type };
            var result = getDrawerTypeForTypeMethod.Invoke(null, parameters) as Type;
            return result != null && typeof(PropertyDrawer).IsAssignableFrom(result);
        }

        /// <summary>
        /// Checks if provided type has an associated <see cref="ToolboxTargetTypeDrawer"/>.
        /// </summary>
        internal static bool HasTargetTypeDrawer(Type type)
        {
            return targetTypeDrawers.ContainsKey(type.IsGenericType ? type.GetGenericTypeDefinition() : type);
        }

        internal static ToolboxDecoratorDrawerBase GetDecoratorDrawer<T>(T attribute) where T : ToolboxDecoratorAttribute
        {
            return GetDecoratorDrawer(attribute.GetType());
        }

        internal static ToolboxDecoratorDrawerBase GetDecoratorDrawer(Type attributeType)
        {
            if (decoratorDrawers.TryGetValue(attributeType, out var drawer))
            {
                return drawer;

            }
            else
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attributeType);
                return null;
            }
        }

        internal static ToolboxConditionDrawerBase GetConditionDrawer<T>(T attribute) where T : ToolboxConditionAttribute
        {
            return GetConditionDrawer(attribute.GetType());
        }

        internal static ToolboxConditionDrawerBase GetConditionDrawer(Type attributeType)
        {
            if (conditionDrawers.TryGetValue(attributeType, out var drawer))
            {
                return drawer;
            }
            else
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attributeType);
                return null;
            }
        }

        internal static ToolboxPropertyDrawerBase GetSelfPropertyDrawer<T>(T attribute) where T : ToolboxSelfPropertyAttribute
        {
            return GetSelfPropertyDrawer(attribute.GetType());
        }

        internal static ToolboxPropertyDrawerBase GetSelfPropertyDrawer(Type attributeType)
        {
            if (selfPropertyDrawers.TryGetValue(attributeType, out var drawer))
            {
                return drawer;
            }
            else
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attributeType);
                return null;
            }
        }

        internal static ToolboxPropertyDrawerBase GetListPropertyDrawer<T>(T attribute) where T : ToolboxListPropertyAttribute
        {
            return GetListPropertyDrawer(attribute.GetType());
        }

        internal static ToolboxPropertyDrawerBase GetListPropertyDrawer(Type attributeType)
        {
            if (listPropertyDrawers.TryGetValue(attributeType, out var drawer))
            {
                return drawer;
            }
            else
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attributeType);
                return null;
            }
        }

        internal static ToolboxTargetTypeDrawer GetTargetTypeDrawer(Type propertyType)
        {
            var targetType = propertyType.IsGenericType
                ? propertyType.GetGenericTypeDefinition()
                : propertyType;
            if (targetTypeDrawers.TryGetValue(targetType, out var drawer))
            {
                return drawer;
            }
            else
            {
                ToolboxEditorLog.LogWarning("There is no type-based drawer associated to the " + propertyType + " type.");
                return null;
            }
        }

        internal static List<Type> GetAllPossibleDecoratorDrawers()
        {
            return decoratorDrawerBase.GetAllChildClasses();
        }

        internal static List<Type> GetAllPossibleConditionDrawers()
        {
            return conditionDrawerBase.GetAllChildClasses();
        }

        internal static List<Type> GetAllPossibleSelfPropertyDrawers()
        {
            return selfPropertyDrawerBase.GetAllChildClasses();
        }

        internal static List<Type> GetAllPossibleListPropertyDrawers()
        {
            return listPropertyDrawerBase.GetAllChildClasses();
        }

        internal static List<Type> GetAllPossibleTargetTypeDrawers()
        {
            return typeof(ToolboxTargetTypeDrawer).GetAllChildClasses();
        }

        /// <summary>
        /// Returns and/or creates (if needed) <see cref="ToolboxPropertyHandler"/> for given property.
        /// </summary>
        internal static ToolboxPropertyHandler GetPropertyHandler(SerializedProperty property)
        {
            if (InspectorUtility.InToolboxEditor)
            {
                //NOTE: maybe type-based key?
                var propertyKey = property.GetPropertyHashKey();
                if (propertyHandlers.TryGetValue(propertyKey, out var propertyHandler))
                {
                    return propertyHandler;
                }
                else
                {
                    return propertyHandlers[propertyKey] = new ToolboxPropertyHandler(property);
                }
            }
            else
            {
                ToolboxEditorLog.LogWarning("Do not use Toolbox-related drawers outside ToolboxEditor.");
                return null;
            }
        }


        internal static bool ToolboxDrawersAllowed { get; set; }

        //TODO:
        //NOTE: unfortunately there is no valid, non-reflection way to check if property has a custom native drawer
        private readonly static MethodInfo getDrawerTypeForTypeMethod =
            ReflectionUtility.GetEditorMethod("UnityEditor.ScriptAttributeUtility", "GetDrawerTypeForType",
                BindingFlags.NonPublic | BindingFlags.Static);
    }
}