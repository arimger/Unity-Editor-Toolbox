using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxSelfPropertyDrawer<T> : ToolboxPropertyDrawer<T> where T : ToolboxSelfPropertyAttribute
    { }
}