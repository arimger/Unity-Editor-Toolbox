using System;

using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class ControlDataStorage<T> : DrawerDataStorage<int, Vector2, Vector2>
    {
        public ControlDataStorage(Func<int, Vector2, Vector2> createMethod) : base(createMethod)
        { }


        protected override string GetKey(int context)
        {
            return context.ToString();
        }


        public int GetControlId()
        {
            return GUIUtility.GetControlID(FocusType.Passive);
        }
    }
}