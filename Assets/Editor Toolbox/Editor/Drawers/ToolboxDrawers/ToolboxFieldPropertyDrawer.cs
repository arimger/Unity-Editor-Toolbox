using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxFieldPropertyDrawer<T> : ToolboxPropertyDrawer<T> where T : ToolboxFieldPropertyAttribute
    { }
}