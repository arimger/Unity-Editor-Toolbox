using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class LineDrawer : OrderedPropertyDrawer<OrderedLineAttribute>
    {
        public override void HandleProperty(SerializedProperty property, OrderedLineAttribute attribute)
        {
            ToolboxEditorUtility.DrawLayoutLine(attribute.Thickness, attribute.Padding);
            base.HandleProperty(property, attribute);
        }
    }
}