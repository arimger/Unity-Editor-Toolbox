using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxDecoratorDrawer<T> : ToolboxDecoratorDrawerBase where T : ToolboxDecoratorAttribute
    {        
        protected virtual void OnGuiBeginSafe(T attribute)
        { }

        protected virtual void OnGuiEndSafe(T attribute)
        { }


        public override sealed void OnGuiBegin(ToolboxAttribute attribute)
        {
            OnGuiBegin(attribute as T);
        }

        public override sealed void OnGuiEnd(ToolboxAttribute attribute)
        {
            OnGuiEnd(attribute as T);
        }


        public void OnGuiBegin(T attribute)
        {
            if (attribute == null)
            {
                return;
            }

            OnGuiBeginSafe(attribute);
        }

        public void OnGuiEnd(T attribute)
        {
            if (attribute == null)
            {
                return;
            }

            OnGuiEndSafe(attribute);
        }
    }
}