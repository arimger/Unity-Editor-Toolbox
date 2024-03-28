using System;
using System.Collections.Generic;

namespace Toolbox.Editor.Drawers
{
    /// <summary>
    /// Internal system responsible for keeping and clearing data between <see cref="UnityEditor.Editor"/>s.
    /// This small system works only for attribute-based drawers and should be defined as a static field.
    /// </summary>
    /// <typeparam name="TKey">Key-related object.</typeparam>
    /// <typeparam name="TData">Data to store.</typeparam>
    /// <typeparam name="TArgs">Any type needed for storage item creation. Pass <see cref="EventArgs.Empty"/> if no additional data is needed.</typeparam>
    public abstract class DrawerDataStorage<TKey, TData, TArgs> : DrawerDataStorageBase
    {
        protected readonly Dictionary<string, TData> items = new Dictionary<string, TData>();

        protected readonly Func<TKey, TArgs, TData> createMethod;
        protected readonly Action<TData> removeMethod;

        public DrawerDataStorage(Func<TKey, TArgs, TData> createMethod) : this(createMethod, null)
        { }

        public DrawerDataStorage(Func<TKey, TArgs, TData> createMethod, Action<TData> removeMethod)
        {
            this.createMethod = createMethod;
            this.removeMethod = removeMethod;
        }

        protected abstract string GetKey(TKey context);

        /// <summary>
        /// Returns and if needed creates new item.
        /// </summary>
        public virtual TData ReturnItem(TKey context, TArgs args)
        {
            var key = GetKey(context);
            if (items.TryGetValue(key, out var item))
            {
                return item;
            }
            else
            {
                return items[key] = CreateItem(context, args);
            }
        }

        public virtual TData CreateItem(TKey context, TArgs args)
        {
            return CreateItem(context, args, true);
        }

        public virtual TData CreateItem(TKey context, TArgs args, bool append)
        {
            var item = createMethod(context, args);
            if (append)
            {
                AppendItem(context, item);
            }

            return item;
        }

        public virtual TData AppendItem(TKey context, TData item)
        {
            var key = GetKey(context);
            return items[key] = item;
        }

        public virtual void ClearItem(TKey context)
        {
            var key = GetKey(context);
            if (removeMethod != null)
            {
                if (items.TryGetValue(key, out var value))
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