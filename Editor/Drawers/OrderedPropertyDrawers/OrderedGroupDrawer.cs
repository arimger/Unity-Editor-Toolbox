using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public abstract class OrderedGroupDrawer<T> : OrderedDrawer<T> where T : OrderedGroupAttribute
    {
        /// <summary>
        /// Collection all grouped properties by single group name.
        /// </summary>
        protected readonly Dictionary<string, List<SerializedProperty>> groupedProperties = new Dictionary<string, List<SerializedProperty>>();


        protected OrderedGroupDrawer(List<SerializedProperty> componentProperties)
        {
            componentProperties.ForEach(property =>
            {
                var attribute = property.GetAttribute<T>();
                if (attribute == null) return;
                var groupName = attribute.GroupName;
                if (groupedProperties.ContainsKey(groupName))
                {
                    groupedProperties[groupName].Add(property);
                    return;
                }

                groupedProperties[groupName] = new List<SerializedProperty>()
                {
                    property
                };
            });
        }

        /// <summary>
        /// Begins group of serialized properties.
        /// </summary>
        /// <param name="attribute"></param>
        protected virtual void OnBeginGroup(T attribute)
        { }

        /// <summary>
        /// Draws all grouped properties.
        /// </summary>
        /// <param name="groupedProperties"></param>
        /// <param name="attribute"></param>
        protected virtual void OnDrawGroup(List<SerializedProperty> groupedProperties, T attribute)
        {
            groupedProperties.ForEach(groupedProperty => base.HandleTargetProperty(groupedProperty, attribute));
        }

        /// <summary>
        /// Ends group of serialized properties.
        /// </summary>
        /// <param name="attribute"></param>
        protected virtual void OnEndGroup(T attribute)
        { }


        /// <summary>
        /// Draws target property(and all other grouped properties) in group
        /// but only if the provided property is previously cached and if this property is first in the desired group.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        public override void HandleTargetProperty(SerializedProperty property, T attribute)
        {
            var groupName = attribute.GroupName;

            if (groupedProperties.ContainsKey(groupName))
            {
                if (groupedProperties[groupName].First().name != property.name) return;

                OnBeginGroup(attribute);
                OnDrawGroup(groupedProperties[groupName], attribute);
                OnEndGroup(attribute);
                return;
            }

            Debug.LogWarning("Non-cached property or " + typeof(T) + " in " + GetType() + ". Property will be drawn in standard way.");
            DrawOrderedProperty(property);
        }
    }
}