using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxCollectionDrawer<T> : ToolboxPropertyDrawerBase<T> where T : ToolboxCollectionAttribute
    {
        public override bool IsPropertyValid(SerializedProperty property)
        {
            //NOTE: this will be always true since the ToolboxPropertyHandler will validate drawer
            return property.isArray;
        } 
    }
}