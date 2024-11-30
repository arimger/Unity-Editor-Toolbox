using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    //TODO:
    //1. dedicated class to initialize and hold drawer-related data
    //2. dedicated class used for settings initialization
    //3. separate logic for resettings active drawers
    //4. validations drawers

    internal static class ToolboxDrawersManager
    {
        [InitializeOnLoadMethod]
        internal static void InitializeModule()
        {
            ToolboxEditorHandler.OnEditorReload += ReloadDrawers;
        }

        private static readonly Type decoratorDrawerBase = typeof(ToolboxDecoratorDrawer<>);
        private static readonly Type conditionDrawerBase = typeof(ToolboxConditionDrawer<>);
        private static readonly Type selfPropertyDrawerBase = typeof(ToolboxSelfPropertyDrawer<>);
        private static readonly Type listPropertyDrawerBase = typeof(ToolboxListPropertyDrawer<>);

        private static readonly Dictionary<Type, ToolboxDecoratorDrawerBase> decoratorDrawers = new Dictionary<Type, ToolboxDecoratorDrawerBase>();
        private static readonly Dictionary<Type, ToolboxConditionDrawerBase> conditionDrawers = new Dictionary<Type, ToolboxConditionDrawerBase>();
        private static readonly Dictionary<Type, ToolboxPropertyDrawerBase> selfPropertyDrawers = new Dictionary<Type, ToolboxPropertyDrawerBase>();
        private static readonly Dictionary<Type, ToolboxPropertyDrawerBase> listPropertyDrawers = new Dictionary<Type, ToolboxPropertyDrawerBase>();

        /// <summary>
        /// Collection of specific drawers mapped to selected (picked) object types.
        /// </summary>
        private static readonly Dictionary<Type, ToolboxTargetTypeDrawer> targetTypeDrawers = new Dictionary<Type, ToolboxTargetTypeDrawer>();

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
                if (drawerType == null || attributeType == null)
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
            ToolboxDrawersManager.settings = settings;

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

            var useDefaultLists = settings.ForceDefaultLists;
            HandleDefaultLists(useDefaultLists);
            var ignoreCustomEditor = !settings.UseToolboxDrawers;
            HandleIgnoreEditor(ignoreCustomEditor);
            //log errors into console only once
            validationEnabled = false;
        }

        /// <summary>
        /// Determines if property has any associated drawer (built-in or custom one).
        /// This method does not take into account <see cref="ToolboxDrawer"/>s.
        /// </summary>
        internal static bool HasNativeTypeDrawer(Type type)
        {
            object[] parameters;
            var parameterInfos = getDrawerTypeForTypeMethod.GetParameters();
            var parametersCount = parameterInfos.Length;
            switch (parametersCount)
            {
                default:
                case 1:
                    parameters = new object[] { type };
                    break;
                //NOTE: Unity 2022.3.23 or above
                case 2:
                    parameters = new object[] { type, false };
                    break;
                //NOTE: Unity 2023.3.x or above
                case 3:
                    parameters = new object[] { type, null, false };
                    break;
            }

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

        internal static void HandleDefaultLists(bool value)
        {
            if (value)
            {
                ScriptingUtility.AppendDefine(ToolboxDefines.defaultListsDefine);
            }
            else
            {
                ScriptingUtility.RemoveDefine(ToolboxDefines.defaultListsDefine);
            }
        }

        internal static void HandleIgnoreEditor(bool value)
        {
            if (value)
            {
                ScriptingUtility.AppendDefine(ToolboxDefines.ignoreEditorDefine);
            }
            else
            {
                ScriptingUtility.RemoveDefine(ToolboxDefines.ignoreEditorDefine);
            }
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
            if (ToolboxEditorHandler.InToolboxEditor)
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

        //TODO: move to utilities
        //NOTE: unfortunately there is no valid, non-reflection way to check if property has a custom native drawer
        private static readonly MethodInfo getDrawerTypeForTypeMethod =
            ReflectionUtility.GetEditorMethod("UnityEditor.ScriptAttributeUtility", "GetDrawerTypeForType",
                BindingFlags.NonPublic | BindingFlags.Static);
    }
}