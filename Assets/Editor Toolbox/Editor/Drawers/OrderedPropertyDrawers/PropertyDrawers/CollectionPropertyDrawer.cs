using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    public class CollectionPropertyDrawer : OrderedPropertyDrawer<ReorderableListAttribute>
    {
        /// <summary>
        /// Stored <see cref="ReorderableList"/> collection by serialized property name.
        /// </summary>
        private readonly Dictionary<string, ReorderableList> reorderableLists = new Dictionary<string, ReorderableList>();


        /// <summary>
        /// Draws <see cref="ReorderableList"/> if provided property is previously cached array/list.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        /// <param name="attribute"></param>
        public override void HandleProperty(SerializedProperty property, ReorderableListAttribute attribute)
        {
            if (!reorderableLists.ContainsKey(property.name))
            {        
                reorderableLists[property.name] = ToolboxEditorUtility.CreateList(property,
                                                    attribute.ListStyle,
                                                    attribute.ElementLabel,
                                                    attribute.FixedSize,
                                                    attribute.Draggable);
            }

            reorderableLists[property.name].DoLayoutList();
        }
    }
}