using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ReorderableListAttributeDrawer : ToolboxListPropertyDrawer<ReorderableListAttribute>
    {
        static ReorderableListAttributeDrawer()
        {
            storage = new DrawerDataStorage<ReorderableList, ReorderableListAttribute>(false, (p, a) =>
            {
                return ToolboxEditorGui.CreateList(p, 
                    a.ListStyle,
                    a.ElementLabel,
                    a.FixedSize,
                    a.Draggable,
                    a.HasHeader);
            });
        }

        private static readonly DrawerDataStorage<ReorderableList, ReorderableListAttribute> storage;


        /// <summary>
        /// Draws a <see cref="ReorderableList"/> if given property was previously cached or creates completely new instance.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ReorderableListAttribute attribute)
        {
            storage.ReturnItem(property, attribute).DoLayoutList();
        }
    }
}