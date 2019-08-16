using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class LineDrawer : OrderedDecoratorDrawer<OrderedLineAttribute>
    {
        public LineDrawer() : base(null)
        { }

        public LineDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        { }


        protected override void OnBeforeProperty(OrderedLineAttribute attribute)
        {
            ToolboxEditorUtility.DrawLayoutLine(attribute.Thickness, attribute.Padding);
        }

        protected override void OnAfterProperty(OrderedLineAttribute attribute)
        { }
    }
}