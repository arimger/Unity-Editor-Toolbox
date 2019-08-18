using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class DisabledOnlyPropertyDrawer : OrderedPropertyDrawer<ReadOnlyAttribute>
    {
        /// <summary>
        /// Drawer method handled by ancestor class.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        /// <param name="attribute"></param>
        public override void HandleProperty(SerializedProperty property, ReadOnlyAttribute attribute)
        {
            EditorGUI.BeginDisabledGroup(true);
            base.HandleProperty(property, attribute);
            EditorGUI.EndDisabledGroup();
        }
    }
}