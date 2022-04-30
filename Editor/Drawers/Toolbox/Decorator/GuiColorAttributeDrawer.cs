using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class GuiColorAttributeDrawer : ToolboxDecoratorDrawer<GuiColorAttribute>
    {
        private Color formerGuiColor;


        protected override void OnGuiBeginSafe(GuiColorAttribute attribute)
        {
            formerGuiColor = GUI.color;
            GUI.color = attribute.Color;
        }

        protected override void OnGuiCloseSafe(GuiColorAttribute attribute)
        {
            GUI.color = formerGuiColor;
        }
    }
}