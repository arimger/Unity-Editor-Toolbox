using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class EndGroupAttributeDrawer : ToolboxDecoratorDrawer<EndGroupAttribute>
    {
        protected override void OnGuiEndSafe(EndGroupAttribute attribute)
        {          
            ToolboxEditorGui.CloseVerticalLayout();
        }
    }
}