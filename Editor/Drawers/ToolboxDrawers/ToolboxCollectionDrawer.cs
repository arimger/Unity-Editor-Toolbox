using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxCollectionDrawer<T> : ToolboxPropertyDrawerBase<T> where T : ToolboxCollectionAttribute
    { }
}