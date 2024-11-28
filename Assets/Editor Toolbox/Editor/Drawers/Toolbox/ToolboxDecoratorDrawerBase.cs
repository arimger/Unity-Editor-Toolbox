using System;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxDecoratorDrawerBase : ToolboxAttributeDrawer
    {
        protected object[] GetDeclaringObjects()
        {
            if (PropertyContext == null)
            {
                return Array.Empty<object>();
            }

            var property = PropertyContext.Property;
            return property.GetDeclaringObjects();
        }

        internal virtual void OnGuiBegin(ToolboxAttribute attribute, ISerializedPropertyContext propertyContext)
        {
            PropertyContext = propertyContext;
            OnGuiBegin(attribute);
            PropertyContext = null;
        }

        internal virtual void OnGuiClose(ToolboxAttribute attribute, ISerializedPropertyContext propertyContext)
        {
            PropertyContext = propertyContext;
            OnGuiClose(attribute);
            PropertyContext = null;
        }

        public abstract void OnGuiBegin(ToolboxAttribute attribute);
        public abstract void OnGuiClose(ToolboxAttribute attribute);

        /// <summary>
        /// Context associated with <see cref="UnityEditor.SerializedProperty"/> that is currently handled.
        /// </summary>
        protected ISerializedPropertyContext PropertyContext { get; private set; }
    }
}