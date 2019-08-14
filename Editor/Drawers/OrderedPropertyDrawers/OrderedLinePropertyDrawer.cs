using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class OrderedLinePropertyDrawer : OrderedPropertyDrawer<OrderedLineAttribute>
    {
        public OrderedLinePropertyDrawer() : base(null)
        { }

        public OrderedLinePropertyDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        { }


        /// <summary>
        /// Drawer method handled by ancestor class.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        protected override void DrawCustomProperty(SerializedProperty property)
        {
            ToolboxEditorUtility.DrawLayoutLine(Attribute.Thickness, Attribute.Padding);
            base.DrawCustomProperty(property);
        }
    }
}