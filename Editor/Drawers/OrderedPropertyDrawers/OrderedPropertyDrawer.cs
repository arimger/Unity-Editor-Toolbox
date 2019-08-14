using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public abstract class OrderedPropertyDrawer<T> : OrderedPropertyDrawerRoot where T : OrderedAttribute
    {
        /// <summary>
        /// All <see cref="SerializedProperty"/> objects which implement the needed <see cref="ComponentAttribute"/>.
        /// </summary>
        protected readonly List<SerializedProperty> targetProperties;


        protected OrderedPropertyDrawer(List<SerializedProperty> componentProperties)
        {
            targetProperties = componentProperties.FindAll(property => property.GetAttribute<T>() != null);
        }


        /// <summary>
        /// Tries to display property excluding all non-target properties.
        /// </summary>
        /// <param name="property"></param>
        public override sealed void HandleProperty(SerializedProperty property)
        {
            if (targetProperties.Any(target => target.name == property.name))
            {
                Attribute = property.GetAttribute<T>();
                DrawCustomProperty(property);
                return;
            }

            Attribute = null;
            DrawDefaultProperty(property);
        }


        public T Attribute { get; private set; }

        public Type AttributeType => typeof(T);
    }
}