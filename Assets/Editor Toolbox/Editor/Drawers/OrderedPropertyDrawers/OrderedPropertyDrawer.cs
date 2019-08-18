using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public abstract class OrderedPropertyDrawer<T> : OrderedDrawerBase where T : OrderedAttribute
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
        public virtual void HandleProperty(SerializedProperty property, T attribute)
        {
            DrawOrderedProperty(property);
        }
    }
}