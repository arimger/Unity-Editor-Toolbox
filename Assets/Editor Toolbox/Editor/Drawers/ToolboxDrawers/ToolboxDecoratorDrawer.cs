using System;

using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxDecoratorDrawer<T> : ToolboxDecoratorDrawerBase where T : ToolboxDecoratorAttribute
    {        
        protected virtual void OnGuiBeginSafe(T attribute)
        { }

        protected virtual void OnGuiCloseSafe(T attribute)
        { }

        [Obsolete("This method is renamed. Override 'OnGuiCloseSafe' instead.")]
        protected virtual void OnGuiEndSafe(T attribute)
        { }


        public override sealed void OnGuiBegin(ToolboxAttribute attribute)
        {
            OnGuiBegin(attribute as T);
        }

        public override sealed void OnGuiClose(ToolboxAttribute attribute)
        {
            OnGuiClose(attribute as T);
        }

        public void OnGuiBegin(T attribute)
        {
            if (attribute == null)
            {
                return;
            }

            OnGuiBeginSafe(attribute);
        }

        public void OnGuiClose(T attribute)
        {
            if (attribute == null)
            {
                return;
            }

            OnGuiCloseSafe(attribute);
        }
    }
}