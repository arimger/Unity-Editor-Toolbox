using UnityEngine;

namespace Toolbox.Editor.Hierarchy
{
    public abstract class HierarchyDataDrawer
    {
        protected GameObject target;


        public virtual void Prepare(GameObject target, Rect availableRect)
        {
            this.target = target;
        }

        public virtual float GetWidth()
        {
            return 17.0f;
        }

        public abstract void OnGui(Rect rect);
    }
}