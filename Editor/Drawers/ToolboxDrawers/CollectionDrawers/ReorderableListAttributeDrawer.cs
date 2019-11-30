using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ReorderableListAttributeDrawer : ToolboxCollectionDrawer<ReorderableListAttribute>
    {
        /// <summary>
        /// Collection of all stored <see cref="ReorderableList"/> instances.
        /// </summary>
        private static Dictionary<string, ReorderableList> listInstances = new Dictionary<string, ReorderableList>();


        /// <summary>
        /// Draws <see cref="ReorderableList"/> if provided property is previously cached array/list or creates completely new instance.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        /// <param name="attribute"></param>
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ReorderableListAttribute attribute)
        {
            var key = property.GetPropertyKey();

            if (!listInstances.TryGetValue(key, out ReorderableList list))
            {
                listInstances[key] = list = ToolboxEditorGui.CreateList(property,
                    attribute.ListStyle,
                    attribute.ElementLabel,
                    attribute.FixedSize,
                    attribute.Draggable);
            }

            list.DoLayoutList(); 
        }

        /// <summary>
        /// Handles data clearing between editors.
        /// </summary>
        public override void OnGuiReload()
        {
            listInstances.Clear();
        }
    }
}