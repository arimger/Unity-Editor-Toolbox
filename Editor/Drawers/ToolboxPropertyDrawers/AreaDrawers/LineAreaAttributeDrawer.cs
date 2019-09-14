namespace Toolbox.Editor.Drawers
{
    public class LineDrawer : ToolboxAreaDrawer<LineAreaAttribute>
    {
        public override void OnGuiBegin(LineAreaAttribute attribute)
        {
            ToolboxEditorUtility.DrawLayoutLine(attribute.Thickness, attribute.Padding);
        }
    }
}