using System;

using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class PropertyDataStorage<T, T1> : DrawerDataStorage<SerializedProperty, T, T1>
    {
        private readonly bool isPersistant;


        public PropertyDataStorage(bool isPersistant, Func<SerializedProperty, T1, T> createMethod) : this(isPersistant, createMethod, null)
        { }

        public PropertyDataStorage(bool isPersistant, Func<SerializedProperty, T1, T> createMethod, Action<T> removeMethod) : base (createMethod, removeMethod)
        {
            this.isPersistant = isPersistant;
        }


        protected override string GetKey(SerializedProperty property)
        {
            return isPersistant
                ? property.GetPropertyTypeKey()
                : property.GetPropertyHashKey();
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