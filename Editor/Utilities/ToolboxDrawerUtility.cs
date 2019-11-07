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

        #region Fields

        private readonly static Dictionary<Type, ToolboxTargetTypeDrawer> targetTypeDrawers = new Dictionary<Type, ToolboxTargetTypeDrawer>();

        private readonly static Dictionary<Type, ToolboxAreaDrawerBase>      decoratorDrawers  = new Dictionary<Type, ToolboxAreaDrawerBase>();
        private readonly static Dictionary<Type, ToolboxPropertyDrawerBase>  propertyDrawers   = new Dictionary<Type, ToolboxPropertyDrawerBase>();
        private readonly static Dictionary<Type, ToolboxPropertyDrawerBase>  collectionDrawers = new Dictionary<Type, ToolboxPropertyDrawerBase>();
        private readonly static Dictionary<Type, ToolboxConditionDrawerBase> conditionDrawers  = new Dictionary<Type, ToolboxConditionDrawerBase>();

        private static readonly Dictionary<string, ToolboxPropertyHandler> propertyHandlers = new Dictionary<string, ToolboxPropertyHandler>();

        #endregion


        private static void CreateAttributeDrawers(IToolboxDrawersSettings settings)
        {
            void AddAttributeDrawer<T>(Type drawerType, Type targetAttributeType, Dictionary<Type, T> drawersCollection) where T : ToolboxDrawer
            {
                if (drawerType == null) return;
                //create desired drawer instance
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
                //validate drawer type reference
                if (drawerType == null)
                {
                    Debug.LogWarning("One of assigned drawer types in ToolboxEditorSettings is empty.");
                    return null;
                }

                //iterate over all base type and find final generic type definition
                while (!drawerType.IsGenericType || drawerType.GetGenericTypeDefinition() != drawerBaseType)
                {
                    if (drawerType.BaseType == null)
                    {
                        return null;
                    }

                    drawerType = drawerType.BaseType;
                }

                //use base definition type to get target attribute type
                return drawerType.IsGenericType ? drawerType.GetGenericArguments().FirstOrDefault() : null;
            }

            for (var i = 0; i < settings.DecoratorDrawersCount; i++)
            {
                var drawerType = settings.GetDecoratorDrawerTypeAt(i);
                var targetType = GetAttributeTargetType(settings.GetDecoratorDrawerTypeAt(i), typeof(ToolboxAreaDrawer<>));
                AddAttributeDrawer(drawerType, targetType, decoratorDrawers);
            }

            for (var i = 0; i < settings.PropertyDrawersCount; i++)
            {
                var drawerType = settings.GetPropertyDrawerTypeAt(i);
                var targetType = GetAttributeTargetType(settings.GetPropertyDrawerTypeAt(i), typeof(ToolboxPropertyDrawer<>));
                AddAttributeDrawer(drawerType, targetType, propertyDrawers);
            }

            for (var i = 0; i < settings.CollectionDrawersCount; i++)
            {
                var drawerType = settings.GetCollectionDrawerTypeAt(i);
                var targetType = GetAttributeTargetType(settings.GetCollectionDrawerTypeAt(i), typeof(ToolboxCollectionDrawer<>));
                AddAttributeDrawer(drawerType, targetType, collectionDrawers);
            }

            for (var i = 0; i < settings.ConditionDrawersCount; i++)
            {
                var drawerType = settings.GetConditionDrawerTypeAt(i);
                var targetType = GetAttributeTargetType(settings.GetConditionDrawerTypeAt(i), typeof(ToolboxConditionDrawer<>));
                AddAttributeDrawer(drawerType, targetType, conditionDrawers);
            }
        }

        private static void CreateTargetTypeDrawers(IToolboxDrawersSettings settings)
        {
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


        internal static void ClearHandlers()
        {
            propertyHandlers.Clear();
        }

        internal static void InitializeDrawers()
        {
            throw new NotImplementedException();
        }

        internal static void InitializeDrawers(IToolboxDrawersSettings settings)
        {
            CreateAttributeDrawers(settings);
            CreateTargetTypeDrawers(settings);
        }


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

        internal static ToolboxAreaDrawerBase    GetDecoratorDrawer<T>(T attribute) where T : ToolboxAreaAttribute
        {
            if (!decoratorDrawers.TryGetValue(attribute.GetType(), out ToolboxAreaDrawerBase drawer))
            {
                throw new AttributeNotSupportedException(attribute);
            }

            return drawer;
        }

        internal static ToolboxPropertyDrawerBase GetPropertyDrawer<T>(T attribute) where T : ToolboxPropertyAttribute
        {
            if (!propertyDrawers.TryGetValue(attribute.GetType(), out ToolboxPropertyDrawerBase drawer))
            {
                throw new AttributeNotSupportedException(attribute);
            }

            return drawer;
        }

        internal static ToolboxPropertyDrawerBase GetCollectionDrawer<T>(T attribute) where T : ToolboxCollectionAttribute
        {
            if (!collectionDrawers.TryGetValue(attribute.GetType(), out ToolboxPropertyDrawerBase drawer))
            {
                throw new AttributeNotSupportedException(attribute);
            }

            return drawer;
        }

        internal static ToolboxConditionDrawerBase GetConditionDrawer<T>(T attribute) where T : ToolboxConditionAttribute
        {
            if (!conditionDrawers.TryGetValue(attribute.GetType(), out ToolboxConditionDrawerBase drawer))
            {
                throw new AttributeNotSupportedException(attribute);
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


        internal static bool ToolboxDrawersAllowed => ToolboxSettingsUtility.Settings.UseToolboxDrawers;


        internal static Action onEditorReload;
    }
}