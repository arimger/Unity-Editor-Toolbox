using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxPropertyDrawer<T> : ToolboxPropertyDrawerBase<T> where T : ToolboxPropertyAttribute
    { }
}