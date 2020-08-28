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
            InspectorUtility.OnEditorReload += ClearHandlers;
        }


        private static readonly MethodInfo getDrawerTypeForTypeMethod = typeof(UnityEditor.Editor).Assembly
                                                                                                  .GetType("UnityEditor.ScriptAttributeUtility")
                                                                                                  .GetMethod("GetDrawerTypeForType", BindingFlags.NonPublic | BindingFlags.Static);

        private readonly static Type targetTypeDrawerBase = typeof(ToolboxTargetTypeDrawer);
        private readonly static Type decoratorDrawerBase  = typeof(ToolboxDecoratorDrawer<>);
        private readonly static Type propertyDrawerBase   = typeof(ToolboxPropertyDrawer<>);
        private readonly static Type collectionDrawerBase = typeof(ToolboxCollectionDrawer<>);
        private readonly static Type conditionDrawerBase  = typeof(ToolboxConditionDrawer<>);

        /// <summary>
        /// Collection of specific drawers associated to particular <see cref="object"/> types.
        /// </summary>
        private readonly static Dictionary<Type, ToolboxTargetTypeDrawer> targetTypeDrawers = new Dictionary<Type, ToolboxTargetTypeDrawer>();

        private readonly static Dictionary<Type, ToolboxDecoratorDrawerBase> decoratorDrawers = new Dictionary<Type, ToolboxDecoratorDrawerBase>();
        private readonly static Dictionary<Type, ToolboxPropertyDrawerBase> propertyDrawers   = new Dictionary<Type, ToolboxPropertyDrawerBase>();
        private readonly static Dictionary<Type, ToolboxPropertyDrawerBase> collectionDrawers = new Dictionary<Type, ToolboxPropertyDrawerBase>();
        private readonly static Dictionary<Type, ToolboxConditionDrawerBase> conditionDrawers = new Dictionary<Type, ToolboxConditionDrawerBase>();

        /// <summary>
        /// Collection of currently cached handlers associated to special key.
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
        /// <param name="settings"></param>
        private static void CreateAttributeDrawers(IToolboxInspectorSettings settings)
        {
            void AddAttributeDrawer<T>(Type drawerType, Type targetAttributeType, Dictionary<Type, T> drawersCollection) where T : ToolboxAttributeDrawer
            {
                if (drawerType == null)
                {
                    return;
                }

                var drawerInstance = Activator.CreateInstance(drawerType) as T;
                if (drawersCollection.ContainsKey(targetAttributeType))
                {
                    if (validationEnabled)
                    {
                        ToolboxEditorLog.LogError("Attribute:" + targetAttributeType + " is associated to more than one ToolboxDrawer.");
                    }

                    return;
                }

                drawersCollection.Add(targetAttributeType, drawerInstance);
            }

            Type GetAttributeTargetType(Type drawerType, Type drawerBaseType)
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
            for (var i = 0; i < settings.DecoratorDrawersCount; i++)
            {
                var drawerType = settings.GetDecoratorDrawerTypeAt(i);
                var targetType = GetAttributeTargetType(settings.GetDecoratorDrawerTypeAt(i), decoratorDrawerBase);
                AddAttributeDrawer(drawerType, targetType, decoratorDrawers);
            }

            propertyDrawers.Clear();
            for (var i = 0; i < settings.PropertyDrawersCount; i++)
            {
                var drawerType = settings.GetPropertyDrawerTypeAt(i);
                var targetType = GetAttributeTargetType(settings.GetPropertyDrawerTypeAt(i), propertyDrawerBase);
                AddAttributeDrawer(drawerType, targetType, propertyDrawers);
            }

            collectionDrawers.Clear();
            for (var i = 0; i < settings.CollectionDrawersCount; i++)
            {
                var drawerType = settings.GetCollectionDrawerTypeAt(i);
                var targetType = GetAttributeTargetType(settings.GetCollectionDrawerTypeAt(i), collectionDrawerBase);
                AddAttributeDrawer(drawerType, targetType, collectionDrawers);
            }

            conditionDrawers.Clear();
            for (var i = 0; i < settings.ConditionDrawersCount; i++)
            {
                var drawerType = settings.GetConditionDrawerTypeAt(i);
                var targetType = GetAttributeTargetType(settings.GetConditionDrawerTypeAt(i), conditionDrawerBase);
                AddAttributeDrawer(drawerType, targetType, conditionDrawers);
            }
        }

        /// <summary>
        /// Creates all possible type-based drawers and add them to the related collection.
        /// </summary>
        /// <param name="settings"></param>
        private static void CreateTargetTypeDrawers(IToolboxInspectorSettings settings)
        {
            targetTypeDrawers.Clear();
            for (var i = 0; i < settings.TargetTypeDrawersCount; i++)
            {
                var drawerType = settings.GetTargetTypeDrawerTypeAt(i);
                if (drawerType == null)
                {
                    if (validationEnabled)
                    {
                        ToolboxEditorLog.LogWarning("One of assigned drawer types in the " + nameof(ToolboxEditorSettings) + " is empty.");
                    }

                    continue;
                }

                var drawerInstance = Activator.CreateInstance(drawerType) as ToolboxTargetTypeDrawer;
                var allTargetTypes = drawerInstance.GetTargetType().GetAllChildClasses();

                foreach (var type in allTargetTypes)
                {
                    if (allTargetTypes.Contains(type))
                    {
                        if (validationEnabled)
                        {
                            ToolboxEditorLog.LogError("Type:" + type + " is associated to more than one ToolboxDrawer.");
                        }

                        continue;
                    }

                    targetTypeDrawers[type] = drawerInstance;
                }
            }
        }


        /// <summary>
        /// Clears all currently cached <see cref="ToolboxPropertyHandler"/>s.
        /// </summary>
        internal static void ClearHandlers()
        {
            propertyHandlers.Clear();
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
        /// <param name="settings"></param>
        internal static void UpdateDrawers(IToolboxInspectorSettings settings)
        {
            ToolboxDrawerModule.settings = settings;

            if (settings == null)
            {
                ToolboxDrawersAllowed = false;
                return;
            }

            ToolboxDrawersAllowed = settings.UseToolboxDrawers;

            CreateAttributeDrawers(settings);
            CreateTargetTypeDrawers(settings);

            validationEnabled = false;
        }


        /// <summary>
        /// Determines if property has any associated drawer (built-in or custom one).
        /// This method does not take into account <see cref="ToolboxDrawer"/>s.
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        internal static bool HasCustomTypeDrawer(Type propertyType)
        {
            var parameters = new object[] { propertyType };
            var result = getDrawerTypeForTypeMethod.Invoke(null, parameters) as Type;
            return result != null && typeof(PropertyDrawer).IsAssignableFrom(result);
        }

        /// <summary>
        /// Checks if provided type has an associated <see cref="ToolboxTargetTypeDrawer"/>.
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        internal static bool HasTargetTypeDrawer(Type propertyType)
        {
            return targetTypeDrawers.ContainsKey(propertyType);
        }


        internal static ToolboxTargetTypeDrawer GetTargetTypeDrawer(Type propertyType)
        {
            if (targetTypeDrawers.TryGetValue(propertyType, out var drawer))
            {
                return drawer;
            }
            else
            {
                return null;
            }    
        }

        internal static ToolboxDecoratorDrawerBase GetDecoratorDrawer<T>(T attribute) where T : ToolboxDecoratorAttribute
        {
            if (decoratorDrawers.TryGetValue(attribute.GetType(), out var drawer))
            {
                return drawer;

            }
            else
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attribute);
                return null;
            }     
        }

        internal static ToolboxPropertyDrawerBase GetPropertyDrawer<T>(T attribute) where T : ToolboxPropertyAttribute
        {
            if (propertyDrawers.TryGetValue(attribute.GetType(), out var drawer))
            {
                return drawer;
            }
            else
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attribute);
                return null;
            }
        }

        internal static ToolboxPropertyDrawerBase GetCollectionDrawer<T>(T attribute) where T : ToolboxCollectionAttribute
        {
            if (collectionDrawers.TryGetValue(attribute.GetType(), out var drawer))
            {
                return drawer;
            }
            else
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attribute);
                return null;
            }     
        }

        internal static ToolboxConditionDrawerBase GetConditionDrawer<T>(T attribute) where T : ToolboxConditionAttribute
        {
            if (conditionDrawers.TryGetValue(attribute.GetType(), out var drawer))
            {
                return drawer;
            }
            else
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attribute);
                return null;
            }    
        }

        internal static List<Type> GetAllPossibleTargetTypeDrawers()
        {
            return targetTypeDrawerBase.GetAllChildClasses();
        }

        internal static List<Type> GetAllPossibleDecoratorDrawers()
        {
            return decoratorDrawerBase.GetAllChildClasses();
        }

        internal static List<Type> GetAllPossiblePropertyDrawers()
        {
            return propertyDrawerBase.GetAllChildClasses();
        }

        internal static List<Type> GetAllPossibleCollectionDrawers()
        {
            return collectionDrawerBase.GetAllChildClasses();
        }

        internal static List<Type> GetAllPossibleConditionDrawers()
        {
            return conditionDrawerBase.GetAllChildClasses();
        }


        /// <summary>
        /// Returns and creates (if needed) <see cref="ToolboxPropertyHandler"/> for given property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static ToolboxPropertyHandler GetPropertyHandler(SerializedProperty property)
        {
            var key = property.GetPropertyKey();

            if (propertyHandlers.TryGetValue(key, out var propertyHandler))
            {
                return propertyHandler;
            }
            else
            {
                return propertyHandlers[key] = propertyHandler = new ToolboxPropertyHandler(property);
            } 
        }


        internal static bool ToolboxDrawersAllowed { get; set; }
    }
}