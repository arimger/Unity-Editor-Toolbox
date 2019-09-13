using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public abstract class OrderedPropertyDrawer<T> : OrderedDrawer<T> where T : ToolboxAttribute
    {
        /// <summary>
        /// Draws target property in provided, custom way.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        public override void HandleProperty(SerializedProperty property, T attribute)
        {
            DrawOrderedProperty(property);
        }
    }
}