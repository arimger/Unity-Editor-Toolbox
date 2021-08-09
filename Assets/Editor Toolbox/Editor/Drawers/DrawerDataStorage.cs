using System;
using System.Collections.Generic;

namespace Toolbox.Editor.Drawers
{
    /// <summary>
    /// Internal system responsible for keeping and clearing data between <see cref="UnityEditor.Editor"/>s.
    /// This small system works only for attribute-based drawers and should be defined as a static field.
    /// </summary>
    /// <typeparam name="T">Key-related object.</typeparam>
    /// <typeparam name="T1">Data to store.</typeparam>
    /// <typeparam name="T2">Any type needed for storage item creation. Pass <see cref="EventArgs.Empty"/> if no additional data is needed.</typeparam>
    public abstract class DrawerDataStorage<T, T1, T2> : DrawerDataStorageBase
    {
        protected readonly Dictionary<string, T1> items = new Dictionary<string, T1>();

        protected readonly Func<T, T2, T1> createMethod;
        protected readonly Action<T1> removeMethod;


        public DrawerDataStorage(Func<T, T2, T1> createMethod) : this(createMethod, null)
        { }

        public DrawerDataStorage(Func<T, T2, T1> createMethod, Action<T1> removeMethod)
        {
            this.createMethod = createMethod;
            this.removeMethod = removeMethod;
        }


        protected abstract string GetKey(T context);


        /// <summary>
        /// Returns and if needed creates new item.
        /// </summary>
        public virtual T1 ReturnItem(T context, T2 args)
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

        public virtual T1 CreateItem(T context, T2 args)
        {
            return CreateItem(context, args, true);
        }

        public virtual T1 CreateItem(T context, T2 args, bool append)
        {
            var item = createMethod(context, args);
            if (append)
            {
                AppendItem(context, item);
            }

            return item;
        }

        public virtual T1 AppendItem(T context, T1 item)
        {
            var key = GetKey(context);
            return items[key] = item;
        }

        public virtual void ClearItem(T context)
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