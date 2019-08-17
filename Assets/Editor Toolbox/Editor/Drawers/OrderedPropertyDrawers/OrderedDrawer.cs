using System;

using UnityEngine;
using UnityEditor;

//TODO: hashset to cache all target properties;

namespace Toolbox.Editor
{
    /// <summary>
    /// Representation of all ordered drawers based on <see cref="OrderedAttribute"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OrderedDrawer<T> : OrderedDrawerBase where T : OrderedAttribute
    {
        /// <summary>
        /// Tries to display provided property using associated attribute.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        public abstract void HandleTargetProperty(SerializedProperty property, T attribute);

        /// <summary>
        /// Tries to display property excluding all non-target properties.
        /// </summary>
        /// <param name="property"></param>
        public override void HandleProperty(SerializedProperty property)
        {
            var attribute = property.GetAttribute<T>();
            if (attribute != null)
            {
                HandleTargetProperty(property, attribute);
                return;
            }

            DrawOrderedProperty(property);
        }


        /// <summary>
        /// Attribute type associated with this drawer.
        /// </summary>
        public Type AttributeType => typeof(T);
    }
}