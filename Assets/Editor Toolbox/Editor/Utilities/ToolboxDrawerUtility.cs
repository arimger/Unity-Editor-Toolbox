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


        private static readonly Dictionary<string, ToolboxPropertyHandler> propertyHandlers = new Dictionary<string, ToolboxPropertyHandler>();


        private readonly static Dictionary<Type, ToolboxAreaDrawerBase> areaDrawers = new Dictionary<Type, ToolboxAreaDrawerBase>();

        private readonly static Dictionary<Type, ToolboxPropertyDrawerBase> propertyDrawers = new Dictionary<Type, ToolboxPropertyDrawerBase>();

        private readonly static Dictionary<Type, ToolboxPropertyDrawerBase> collectionDrawers = new Dictionary<Type, ToolboxPropertyDrawerBase>();

        private readonly static Dictionary<Type, ToolboxConditionDrawerBase> conditionDrawers = new Dictionary<Type, ToolboxConditionDrawerBase>();


        internal static void ClearHandlers()
        {
            propertyHandlers.Clear();
        }

        internal static void InitializeDrawers()
        {
            throw new NotImplementedException();
        }

        internal static void InitializeDrawers(ToolboxEditorSettings settings)
        {
            //local method used in drawer creation
            void CreateDrawer<T>(Type drawerType, Type targetAttributeType, Dictionary<Type, T> drawersCollection) where T : ToolboxDrawer
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

            //local method used in search for proper target attributes
            Type GetTargetType(Type drawerType, Type drawerBaseType)
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


            //iterate over all assigned all possible drawers, create them and store
            for (var i = 0; i < settings.AreaDrawersCount; i++)
            {
                var drawerType = settings.GetAreaDrawerTypeAt(i);
                var targetType = GetTargetType(settings.GetAreaDrawerTypeAt(i), typeof(ToolboxAreaDrawer<>));
                CreateDrawer(drawerType, targetType, areaDrawers);
            }

            for (var i = 0; i < settings.PropertyDrawersCount; i++)
            {
                var drawerType = settings.GetPropertyDrawerTypeAt(i);
                var targetType = GetTargetType(settings.GetPropertyDrawerTypeAt(i), typeof(ToolboxPropertyDrawer<>));
                CreateDrawer(drawerType, targetType, propertyDrawers);
            }

            for (var i = 0; i < settings.CollectionDrawersCount; i++)
            {
                var drawerType = settings.GetCollectionDrawerTypeAt(i);
                var targetType = GetTargetType(settings.GetCollectionDrawerTypeAt(i), typeof(ToolboxCollectionDrawer<>));
                CreateDrawer(drawerType, targetType, collectionDrawers);
            }

            for (var i = 0; i < settings.ConditionDrawersCount; i++)
            {
                var drawerType = settings.GetConditionDrawerTypeAt(i);
                var targetType = GetTargetType(settings.GetConditionDrawerTypeAt(i), typeof(ToolboxConditionDrawer<>));
                CreateDrawer(drawerType, targetType, conditionDrawers);
            }
        }

        internal static ToolboxAreaDrawerBase GetAreaDrawer<T>(T attribute) where T : ToolboxAreaAttribute
        {
            if (!areaDrawers.TryGetValue(attribute.GetType(), out ToolboxAreaDrawerBase drawer))
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


        internal static bool ToolboxDrawersAllowed => ToolboxSettingsUtility.ToolboxDrawersAllowed;


        internal static Action onEditorReload;
    }
}