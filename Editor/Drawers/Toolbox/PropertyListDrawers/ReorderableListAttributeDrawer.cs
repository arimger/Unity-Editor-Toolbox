using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ReorderableListAttributeDrawer : ToolboxListPropertyDrawer<ReorderableListAttribute>
    {
        static ReorderableListAttributeDrawer()
        {
            storage = new PropertyDataStorage<ReorderableListBase, ReorderableListAttribute>(false, (p, a) =>
            {
                return ToolboxEditorGui.CreateList(p,
                    a.ListStyle,
                    a.ElementLabel,
                    a.FixedSize,
                    a.Draggable,
                    a.HasHeader,
                    a.HasLabels,
                    a.Foldable);
            });
        }

        private static readonly PropertyDataStorage<ReorderableListBase, ReorderableListAttribute> storage;


        /// <summary>
        /// Draws a <see cref="ReorderableList"/> if given property was previously cached or creates completely new instance.
        /// </summary>
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ReorderableListAttribute attribute)
        {
            storage.ReturnItem(property, attribute).DoList();
        }
    }
}