using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public abstract class OrderedPresetDrawer<T> : OrderedDrawerBase where T : OrderedAttribute
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
        /// Tries to draw property only if is currently cached.
        /// </summary>
        /// <param name="property"></param>
        public override void HandleProperty(SerializedProperty property)
        {
            if (targetProperties.Any(targetProperty => SerializedProperty.EqualContents(targetProperty, property)))
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
        public virtual void HandleProperty(SerializedProperty property, T attribute)
        {
            DrawOrderedProperty(property);
        }
    }
}