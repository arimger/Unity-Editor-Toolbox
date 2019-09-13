namespace Toolbox.Editor.Drawers
{
    public abstract  class ToolboxAreaDrawerBase
    {
        public virtual void OnGuiBegin()
        { }

        public virtual void OnGuiBegin(ToolboxAttribute attribute)
        {
            OnGuiBegin();
        }

        public virtual void OnGuiEnd()
        { }

        public virtual void OnGuiEnd(ToolboxAttribute attribute)
        {
            OnGuiEnd();
        }
    }
}