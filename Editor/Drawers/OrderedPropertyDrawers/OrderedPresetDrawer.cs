using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    /// <summary>
    /// Constructor-based drawer, needs list of <see cref="SerializedProperty"/> objects for data preset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OrderedPresetDrawer<T> : OrderedDrawer<T> where T : OrderedAttribute
    {
        /// <summary>
        /// Cached all target properties during preset.
        /// </summary>
        protected readonly List<SerializedProperty> targetProperties = new List<SerializedProperty>();


        protected OrderedPresetDrawer(List<SerializedProperty> componentProperties)
        {
            componentProperties.ForEach(property =>
            {
                if (property.GetAttribute<T>() == null) return;
                targetProperties.Add(property);
            });
        }


        /// <summary>
        /// Checks if property was previously cached.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected virtual bool IsCached(SerializedProperty property)
        {
            return targetProperties.Any(targetProperty => SerializedProperty.EqualContents(targetProperty, property));
        }


        /// <summary>
        /// Tries to draw property only if is currently cached.
        /// </summary>
        /// <param name="property"></param>
        public override void HandleProperty(SerializedProperty property)
        {
            if (IsCached(property))
            {
                HandleProperty(property, property.GetAttribute<T>());
                return;
            }

            DrawOrderedProperty(property);
        }

        /// <summary>
        /// Draws target property in provided, custom way.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        public override void HandleProperty(SerializedProperty property, T attribute)
        {
            DrawOrderedProperty(property);
        }
    }
}