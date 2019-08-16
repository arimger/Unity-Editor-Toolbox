using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public abstract class OrderedDecoratorDrawer<T> : OrderedDrawer<T> where T : OrderedAttribute
    {
        protected OrderedDecoratorDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        { }


        protected abstract void OnBeforeProperty(T attribute);
        protected abstract void OnAfterProperty(T attribute);


        /// <summary>
        /// Draws target property with additional decorators.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        public override void HandleTargetProperty(SerializedProperty property, T attribute)
        {
            OnBeforeProperty(attribute);
            base.HandleTargetProperty(property, attribute);
            OnAfterProperty(attribute);
        }
    }
}