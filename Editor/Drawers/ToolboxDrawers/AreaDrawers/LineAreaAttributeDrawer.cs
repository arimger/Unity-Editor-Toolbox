namespace Toolbox.Editor.Drawers
{
    public class LineAreaAttributeDrawer : ToolboxAreaDrawer<LineAreaAttribute>
    {
        public override void OnGuiBegin(LineAreaAttribute attribute)
        {
            ToolboxEditorGui.DrawLayoutLine(attribute.Thickness, attribute.Padding);
        }
    }
}