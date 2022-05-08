using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxListPropertyDrawer<T> : ToolboxPropertyDrawer<T> where T : ToolboxListPropertyAttribute
    {
        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.isArray;
        }
    }
}