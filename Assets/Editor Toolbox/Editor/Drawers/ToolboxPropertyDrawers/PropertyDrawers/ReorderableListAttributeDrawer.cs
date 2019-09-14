using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ReorderableListAttributeDrawer : ToolboxPropertyDrawer<ReorderableListAttribute>
    {
        //NOTE: we have to clear cache when components/selection change;
        [InitializeOnLoadMethod]
        public static void InitializeDrawer()
        {
            Selection.selectionChanged += () =>
            {
                listInstances.Clear();
            };
        }


        /// <summary>
        /// Collection of all stored <see cref="ReorderableList"/> instances by its <see cref="SerializedObject"/>.
        /// </summary>
        private readonly static Dictionary<string, ReorderableList> listInstances = new Dictionary<string, ReorderableList>();


        /// <summary>
        /// Draws <see cref="ReorderableList"/> if provided property is previously cached array/list.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        /// <param name="attribute"></param>
        public override void OnGui(SerializedProperty property, ReorderableListAttribute attribute)
        {
            var key = property.serializedObject.targetObject.GetInstanceID() + "-" + property.name;
            
            if (!listInstances.ContainsKey(key))
            {
                listInstances[key] = ToolboxEditorUtility.CreateList(property,
                    attribute.ListStyle,
                    attribute.ElementLabel,
                    attribute.FixedSize,
                    attribute.Draggable);
            }

            listInstances[key].DoLayoutList();
        }
    }
}