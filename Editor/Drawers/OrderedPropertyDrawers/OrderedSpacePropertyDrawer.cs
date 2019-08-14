using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class OrderedSpacePropertyDrawer : OrderedPropertyDrawer<OrderedSpaceAttribute>
    {
        public OrderedSpacePropertyDrawer() : base(null)
        { }

        public OrderedSpacePropertyDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        { }


        /// <summary>
        /// Drawer method handled by ancestor class.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        protected override void DrawCustomProperty(SerializedProperty property)
        {
            GUILayout.Space(Attribute.SpaceBefore);
            base.DrawCustomProperty(property);
            GUILayout.Space(Attribute.SpaceAfter);
        }
    }
}