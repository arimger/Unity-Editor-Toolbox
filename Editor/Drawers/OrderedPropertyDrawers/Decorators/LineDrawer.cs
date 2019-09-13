using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class LineDrawer : OrderedPropertyDrawer<ToolboxLineAttribute>
    {
        public override void HandleProperty(SerializedProperty property, ToolboxLineAttribute attribute)
        {
            ToolboxEditorUtility.DrawLayoutLine(attribute.Thickness, attribute.Padding);
            base.HandleProperty(property, attribute);
        }
    }
}