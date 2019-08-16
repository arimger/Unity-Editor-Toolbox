using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public abstract class OrderedPropertyDrawer<T> : OrderedDrawer<T> where T : OrderedAttribute
    {
        protected OrderedPropertyDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        { }


        /// <summary>
        /// Draws target property in provided, custom way.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        public override void HandleTargetProperty(SerializedProperty property, T attribute)
        {
            base.HandleTargetProperty(property, attribute);
        }
    }
}