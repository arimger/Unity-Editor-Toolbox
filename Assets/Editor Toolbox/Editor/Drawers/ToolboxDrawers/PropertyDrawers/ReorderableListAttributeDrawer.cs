using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ReorderableListAttributeDrawer : ToolboxPropertyDrawer<ReorderableListAttribute>
    {
        //NOTE: we have to clear cache when components/selection change;
        [InitializeOnLoadMethod]
        private static void InitializeDrawer()
        {
            Selection.selectionChanged += DeinitializeDrawer;
        }

        private static void DeinitializeDrawer()
        {
            listInstances.Clear();
        }

        private static string GenerateKey(SerializedProperty property)
        {
            //TODO: check whenever hash code is outdated and reinitialize drawer
            return property.serializedObject.GetHashCode() + "-" +
                   property.serializedObject.targetObject.GetInstanceID() + "-" + property.name;
        }


        /// <summary>
        /// Collection of all stored <see cref="ReorderableList"/> instances by its <see cref="SerializedObject"/>.
        /// </summary>
        private static Dictionary<string, ReorderableList> listInstances = new Dictionary<string, ReorderableList>();


        /// <summary>
        /// Draws <see cref="ReorderableList"/> if provided property is previously cached array/list.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        /// <param name="attribute"></param>
        public override void OnGui(SerializedProperty property, ReorderableListAttribute attribute)
        {
            var key = GenerateKey(property);

            if (!listInstances.TryGetValue(key, out ReorderableList list))
            {
                list = listInstances[key] = ToolboxEditorUtility.CreateList(property,
                    attribute.ListStyle,
                    attribute.ElementLabel,
                    attribute.FixedSize,
                    attribute.Draggable);
            }

            list.DoLayoutList(); 
        }
    }
}