using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ReorderableListAttributeDrawer : ToolboxPropertyDrawer<ReorderableListAttribute>
    {
        /// <summary>
        /// Collection of all stored <see cref="ReorderableList"/> instances by its <see cref="SerializedObject"/>.
        /// </summary>
        private Dictionary<SerializedObject, ReorderableList> reorderableLists = new Dictionary<SerializedObject, ReorderableList>();


        /// <summary>
        /// Draws <see cref="ReorderableList"/> if provided property is previously cached array/list.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        /// <param name="attribute"></param>
        public override void OnGui(SerializedProperty property, ReorderableListAttribute attribute)
        {
            var key = property.serializedObject;
            if (!reorderableLists.ContainsKey(key))
            {
                reorderableLists[key] = ToolboxEditorUtility.CreateList(property,
                    attribute.ListStyle,
                    attribute.ElementLabel,
                    attribute.FixedSize,
                    attribute.Draggable);
            }

            reorderableLists[key].DoLayoutList();
        }
    }
}