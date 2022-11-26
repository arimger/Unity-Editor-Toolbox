using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class EndVerticalAttributeDrawer : ToolboxDecoratorDrawer<EndVerticalAttribute>
    {
        protected override void OnGuiCloseSafe(EndVerticalAttribute attribute)
        {
            ToolboxLayoutHandler.CloseVertical();
        }
    }
}