using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class LineAreaAttributeDrawer : ToolboxDecoratorDrawer<LineAreaAttribute>
    {
        protected override void OnGuiBeginSafe(LineAreaAttribute attribute)
        {
            ToolboxEditorGui.DrawLayoutLine(attribute.Thickness, attribute.Padding);
        }
    }
}