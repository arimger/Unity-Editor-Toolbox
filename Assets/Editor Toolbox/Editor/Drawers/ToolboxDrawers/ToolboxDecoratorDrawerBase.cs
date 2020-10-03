using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract  class ToolboxDecoratorDrawerBase : ToolboxAttributeDrawer
    {
        public abstract void OnGuiBegin(ToolboxAttribute attribute);

        public abstract void OnGuiClose(ToolboxAttribute attribute);
    }
}