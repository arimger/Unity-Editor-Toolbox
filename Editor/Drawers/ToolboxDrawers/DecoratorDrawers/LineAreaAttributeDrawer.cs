using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class LineAreaAttributeDrawer : ToolboxDecoratorDrawer<LineAreaAttribute>
    {
        public override void OnGuiBegin(LineAreaAttribute attribute)
        {
            ToolboxEditorGui.DrawLayoutLine(attribute.Thickness, attribute.Padding);
        }
    }
}