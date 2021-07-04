using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class LineAttributeDrawer : ToolboxDecoratorDrawer<LineAttribute>
    {
        protected override void OnGuiBeginSafe(LineAttribute attribute)
        {
            ToolboxEditorGui.DrawLine(attribute.Thickness,
                                      attribute.Padding,
                                      attribute.GuiColor,
                                      attribute.IsHorizontal,
                                      attribute.ApplyIndent);
        }
    }
}