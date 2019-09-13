using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class DisabledOnlyPropertyDrawer : OrderedPropertyDrawer<DisableAttribute>
    {
        /// <summary>
        /// Drawer method handled by ancestor class.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        /// <param name="attribute"></param>
        public override void HandleProperty(SerializedProperty property, DisableAttribute attribute)
        {
            EditorGUI.BeginDisabledGroup(true);
            base.HandleProperty(property, attribute);
            EditorGUI.EndDisabledGroup();
        }
    }
}