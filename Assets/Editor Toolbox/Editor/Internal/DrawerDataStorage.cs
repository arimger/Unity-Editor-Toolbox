using System;
using System.Collections.Generic;

using UnityEditor;

namespace Toolbox.Editor.Internal
{
    abstract class DrawerDataStorage
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


        protected DrawerDataStorage()
        {
            storages.Add(this);
        }


        //TODO: clean up
        private static readonly List<DrawerDataStorage> storages = new List<DrawerDataStorage>();


        public abstract void ClearItems();
    }

    /// <summary>
    /// Internal system responsible for keeping and clearing data between <see cref="UnityEditor.Editor"/>s.
    /// This small system works only for attribute-based drawers and should be defined as a static field.
    /// </summary>
    /// <typeparam name="T">Data to store.</typeparam>
    /// <typeparam name="T1">Any type needed for storage item creation. Pass <see cref="EventArgs.Empty"/> if no additional data is needed.</typeparam>
    internal class DrawerDataStorage<T, T1> : DrawerDataStorage
    {
        internal DrawerDataStorage(bool isPersistant, Func<SerializedProperty, T1, T> createMethod) : this(isPersistant, createMethod, null)
        { }

        internal DrawerDataStorage(bool isPersistant, Func<SerializedProperty, T1, T> createMethod, Action<T> removeMethod)
        {
            this.isPersistant = isPersistant;
            this.createMethod = createMethod;
            this.removeMethod = removeMethod;
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

        public T ReturnItem(SerializedProperty property, T1 args)
        {
            var key = GetKey(property);
            if (items.TryGetValue(key, out T item))
            {
                return item;
            }
            else
            {
                return items[key] = CreateItem(property, args);
            }
        }

        public T CreateItem(SerializedProperty property, T1 args)
        {
            return createMethod(property, args);
        }

        public T ApplyItem(SerializedProperty property, T1 args)
        {
            return ApplyItem(property, createMethod(property, args));
        }

        public T ApplyItem(SerializedProperty property, T item)
        {
            var key = GetKey(property);
            return items[key] = item;
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

        public override void ClearItems()
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