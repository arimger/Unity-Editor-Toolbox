using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    internal static class ToolboxDrawerUtility
    {
        [InitializeOnLoadMethod]
        internal static void InitializeEvents()
        {
            Selection.selectionChanged += onEditorReload + ClearHandlers;
        }


        private readonly static Type targetTypeDrawerBase = typeof(ToolboxTargetTypeDrawer);
        private readonly static Type decoratorDrawerBase = typeof(ToolboxDecoratorDrawer<>);
        private readonly static Type propertyDrawerBase = typeof(ToolboxPropertyDrawer<>);
        private readonly static Type collectionDrawerBase = typeof(ToolboxCollectionDrawer<>);
        private readonly static Type conditionDrawerBase = typeof(ToolboxConditionDrawer<>);

        private readonly static Dictionary<Type, ToolboxTargetTypeDrawer> targetTypeDrawers = new Dictionary<Type, ToolboxTargetTypeDrawer>();

        private readonly static Dictionary<Type, ToolboxDecoratorDrawerBase> decoratorDrawers  = new Dictionary<Type, ToolboxDecoratorDrawerBase>();
        private readonly static Dictionary<Type, ToolboxPropertyDrawerBase>  propertyDrawers   = new Dictionary<Type, ToolboxPropertyDrawerBase>();
        private readonly static Dictionary<Type, ToolboxPropertyDrawerBase>  collectionDrawers = new Dictionary<Type, ToolboxPropertyDrawerBase>();
        private readonly static Dictionary<Type, ToolboxConditionDrawerBase> conditionDrawers  = new Dictionary<Type, ToolboxConditionDrawerBase>();

        /// <summary>
        /// Collection to store current handlers associated to special key.
        /// </summary>
        private static readonly Dictionary<string, ToolboxPropertyHandler> propertyHandlers = new Dictionary<string, ToolboxPropertyHandler>();


        /// <summary>
        /// Settings provided to handle custom drawers.
        /// </summary>
        private static IToolboxDrawersSettings settings;


        /// <summary>
        /// Creates all possible attribute-based drawers and add them to proper collections.
        /// </summary>
        /// <param name="settings"></param>
        private static void CreateAttributeDrawers(IToolboxDrawersSettings settings)
        {
            void AddAttributeDrawer<T>(Type drawerType, Type targetAttributeType, Dictionary<Type, T> drawersCollection) where T : ToolboxAttributeDrawer
            {
                if (drawerType == null) return;
                var drawerInstance = Activator.CreateInstance(drawerType) as T;

                if (drawersCollection.ContainsKey(targetAttributeType))
                {
                    Debug.LogWarning(targetAttributeType + " is already associated to more than one ToolboxDrawer.");
                    return;
                }

                drawersCollection.Add(targetAttributeType, drawerInstance);
            }

            Type GetAttributeTargetType(Type drawerType, Type drawerBaseType)
            {
                if (drawerType == null)
                {
                    Debug.LogWarning("One of assigned drawer types in " + nameof(ToolboxEditorSettings) + " is empty.");
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
                var targetType = GetAttributeTargetType(settings.GetConditionDrawerTypeAt(i),conditionDrawerBase);
                AddAttributeDrawer(drawerType, targetType, conditionDrawers);
            }
        }

        /// <summary>
        /// Creates all possible type-based drawers and add them ro proper collection.
        /// </summary>
        /// <param name="settings"></param>
        private static void CreateTargetTypeDrawers(IToolboxDrawersSettings settings)
        {
            targetTypeDrawers.Clear();
            for (var i = 0; i < settings.TargetTypeDrawersCount; i++)
            {
                var drawerType = settings.GetTargetTypeDrawerTypeAt(i);
                if (drawerType == null) continue;
                var drawerInstance = Activator.CreateInstance(drawerType) as ToolboxTargetTypeDrawer;
                var targetTypes = drawerInstance.GetTargetType().GetAllChildClasses();

                foreach (var type in targetTypes)
                {
                    targetTypeDrawers[type] = drawerInstance;
                }
            }
        }


        /// <summary>
        /// Clears all currently stored <see cref="ToolboxPropertyHandler"/>s.
        /// This method is always called through <see cref="onEditorReload"/> event.
        /// </summary>
        internal static void ClearHandlers()
        {
            propertyHandlers.Clear();
        }

        /// <summary>
        /// Initializes all possible drawers.
        /// </summary>
        internal static void PerformData()
        {
            if (settings == null)
            {
                return;
            }

            PerformData(settings);
        }

        /// <summary>
        /// Initializes all assigned drawers using provided settings reference.
        /// </summary>
        /// <param name="settings"></param>
        internal static void PerformData(IToolboxDrawersSettings settings)
        {
            ToolboxDrawerUtility.settings = settings;

            CreateAttributeDrawers(settings);
            CreateTargetTypeDrawers(settings);

            ToolboxDrawersAllowed = settings.UseToolboxDrawers;
        }


        /// <summary>
        /// Checks if provided type has <see cref="ToolboxTargetTypeDrawer"/>.
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        internal static bool HasTargetTypeDrawer(Type propertyType)
        {
            return targetTypeDrawers.ContainsKey(propertyType);
        }


        internal static ToolboxTargetTypeDrawer GetTargetTypeDrawer(Type propertyType)
        {
            if (!targetTypeDrawers.TryGetValue(propertyType, out ToolboxTargetTypeDrawer drawer))
            {
                return null;
            }

            return drawer;
        }

        internal static ToolboxDecoratorDrawerBase GetDecoratorDrawer<T>(T attribute) where T : ToolboxDecoratorAttribute
        {
            if (!decoratorDrawers.TryGetValue(attribute.GetType(), out ToolboxDecoratorDrawerBase drawer))
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attribute);
                return null;
            }

            return drawer;
        }

        internal static ToolboxPropertyDrawerBase GetPropertyDrawer<T>(T attribute) where T : ToolboxPropertyAttribute
        {
            if (!propertyDrawers.TryGetValue(attribute.GetType(), out ToolboxPropertyDrawerBase drawer))
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attribute);
                return null;
            }

            return drawer;
        }

        internal static ToolboxPropertyDrawerBase GetCollectionDrawer<T>(T attribute) where T : ToolboxCollectionAttribute
        {
            if (!collectionDrawers.TryGetValue(attribute.GetType(), out ToolboxPropertyDrawerBase drawer))
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attribute);
                return null;
            }

            return drawer;
        }

        internal static ToolboxConditionDrawerBase GetConditionDrawer<T>(T attribute) where T : ToolboxConditionAttribute
        {
            if (!conditionDrawers.TryGetValue(attribute.GetType(), out ToolboxConditionDrawerBase drawer))
            {
                ToolboxEditorLog.AttributeNotSupportedWarning(attribute);
                return null;
            }

            return drawer;
        }

        internal static ToolboxPropertyHandler GetPropertyHandler(SerializedProperty property)
        {
            var key = property.GetPropertyKey();

            if (!propertyHandlers.TryGetValue(key, out ToolboxPropertyHandler propertyHandler))
            {
                return propertyHandlers[key] = propertyHandler = new ToolboxPropertyHandler(property);
            }

            return propertyHandler;
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


        internal static bool ToolboxDrawersAllowed { get; private set; }


        /// <summary>
        /// Action called every time when the inspector window is fully rebuilt.
        /// </summary>
        internal static Action onEditorReload;
    }
}