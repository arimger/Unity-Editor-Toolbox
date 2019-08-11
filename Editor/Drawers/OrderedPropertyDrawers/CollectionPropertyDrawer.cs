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
        private Dictionary<string, ReorderableList> reorderableLists = new Dictionary<string, ReorderableList>();


        public CollectionPropertyDrawer() : base(null)
        { }

        public CollectionPropertyDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        {
            targetProperties.ForEach(reorderableProperty =>
            {
                var attribute = reorderableProperty.GetAttribute<ReorderableListAttribute>();
                var list = ToolboxEditorUtility.CreateList(reorderableProperty,
                    attribute.ListStyle,
                    attribute.ElementLabel,
                    attribute.FixedSize,
                    attribute.Draggable);
                reorderableLists.Add(reorderableProperty.name, list);
            });
        }


        /// <summary>
        /// Drawer method handled by ancestor class.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        protected override void DrawCustomProperty(SerializedProperty property)
        {
            if (reorderableLists.ContainsKey(property.name))
            {
                reorderableLists[property.name].DoLayoutList();
                return;
            }

            base.DrawCustomProperty(property);
        }
    }
}