using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    internal class DrawerDataStorage<T, T1> where T1 : ToolboxAttribute
    {
        static DrawerDataStorage()
        {
            InspectorUtility.OnEditorReload += () =>
            {
                for (var i = 0; i < storages.Count; i++)
                {
                    storages[i].ClearItems();
                }
            };
        }

        private static readonly List<DrawerDataStorage<T, T1>> storages = new List<DrawerDataStorage<T, T1>>();


        public DrawerDataStorage(bool isPersistant, Func<SerializedProperty, T1, T> createMethod) : this(isPersistant, createMethod, null)
        { }

        public DrawerDataStorage(bool isPersistant, Func<SerializedProperty, T1, T> createMethod, Action<T> removeMethod)
        {
            this.isPersistant = isPersistant;
            this.createMethod = createMethod;
            this.removeMethod = removeMethod;

            storages.Add(this);
        }


        private readonly bool isPersistant;

        private readonly Dictionary<string, T> items = new Dictionary<string, T>();

        private readonly Func<SerializedProperty, T1, T> createMethod;
        private readonly Action<T> removeMethod;


        private string GetKey(SerializedProperty property)
        {
            return isPersistant
                ? property.GetPropertyTypeKey()
                : property.GetPropertyHashKey();
        }

        public T ReturnItem(SerializedProperty property, T1 attribute)
        {
            var key = GetKey(property);

            if (items.TryGetValue(key, out T item))
            {
                return item;
            }
            else
            {
                return items[key] = createMethod(property, attribute);
            }
        }

        public void ApplyItem(SerializedProperty property, T item)
        {
            items[GetKey(property)] = item;
        }

        public void ClearItem(SerializedProperty property)
        {
            var key = GetKey(property);

            if (removeMethod != null)
            {
                if (items.TryGetValue(key, out T value))
                {
                    removeMethod(value);
                    items.Remove(key);
                }
            }
            else
            {
                items.Remove(key);
            }
        }

        public void ClearItems()
        {
            if (isPersistant)
            {
                return;
            }
            else
            {
                if (removeMethod != null)
                {
                    foreach (var item in items.Values)
                    {
                        removeMethod(item);
                    }
                }

                items.Clear();
            }
        }
    }
}