using System;

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
        /// Tries to display property excluding all non-target properties.
        /// </summary>
        /// <param name="property"></param>
        public override void HandleProperty(SerializedProperty property)
        {
            var attribute = property.GetAttribute<T>();
            if (attribute != null)
            {
                HandleProperty(property, attribute);
                return;
            }

            DrawOrderedProperty(property);
        }

        /// <summary>
        /// Draws target property in provided, custom way.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        public abstract void HandleProperty(SerializedProperty property, T attribute);


        /// <summary>
        /// Attribute type associated with this drawer.
        /// </summary>
        public Type AttributeType => typeof(T);
    }
}