using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class LineDrawer : OrderedPropertyDrawer<OrderedLineAttribute>
    {
        public override void HandleTargetProperty(SerializedProperty property, OrderedLineAttribute attribute)
        {
            ToolboxEditorUtility.DrawLayoutLine(attribute.Thickness, attribute.Padding);
            base.HandleTargetProperty(property, attribute);
        }
    }
}