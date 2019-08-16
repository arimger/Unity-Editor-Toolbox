using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    /// <summary>
    /// Representation of all ordered drawers based on <see cref="OrderedAttribute"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OrderedDrawer<T> : OrderedDrawerBase where T : OrderedAttribute
    {
        /// <summary>
        /// All <see cref="SerializedProperty"/> objects which implements the needed <see cref="OrderedAttribute"/>.
        /// </summary>
        protected readonly List<SerializedProperty> targetProperties;


        protected OrderedDrawer(List<SerializedProperty> componentProperties)
        {
            targetProperties = componentProperties.FindAll(property => property.GetAttribute<T>() != null);
        }


        /// <summary>
        /// Tries to display provided property using associated attribute.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        public virtual void HandleTargetProperty(SerializedProperty property, T attribute)
        {
            DrawOrderedProperty(property);
        }

        /// <summary>
        /// Tries to display property excluding all non-target properties.
        /// </summary>
        /// <param name="property"></param>
        public override void HandleProperty(SerializedProperty property)
        {
            if (targetProperties.Any(target => target.name == property.name))
            {
                HandleTargetProperty(property, property.GetAttribute<T>());
                return;
            }

            DrawOrderedProperty(property);
        }


        public Type AttributeType => typeof(T);
    }
}