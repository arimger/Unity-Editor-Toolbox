using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class EndTabGroupAttributeDrawer : ToolboxDecoratorDrawer<EndTabGroupAttribute>
    {
        protected override void OnGuiCloseSafe(EndTabGroupAttribute attribute)
        {
            ToolboxLayoutHandler.CloseVertical();
        }
    }
}