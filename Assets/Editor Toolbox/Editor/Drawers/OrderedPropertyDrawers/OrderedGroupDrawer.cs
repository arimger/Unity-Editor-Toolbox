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


        protected OrderedGroupDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        {
            targetProperties.ForEach(property =>
            {
                var groupName = property.GetAttribute<T>().GroupName;
                if (groupedProperties.ContainsKey(groupName))
                {
                    groupedProperties[groupName].Add(property);
                }
                else
                {
                    groupedProperties[groupName] = new List<SerializedProperty>()
                    {
                        property
                    };
                }
            });
        }


        protected virtual void OnBeginGroup(T attribute)
        { }

        protected virtual void OnDrawGroup(List<SerializedProperty> groupedProperties, T attribute)
        {
            groupedProperties.ForEach(groupedProperty => base.HandleTargetProperty(groupedProperty, attribute));
        }

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

            if (groupedProperties[groupName].FirstOrDefault().name != property.name) return;

            OnBeginGroup(attribute);
            OnDrawGroup(groupedProperties[groupName], attribute);
            OnEndGroup(attribute);
        }
    }
}