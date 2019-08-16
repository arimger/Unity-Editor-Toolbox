using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    /// <summary>
    /// Basic abstract representation of all ordered drawers.
    /// </summary>
    public abstract class OrderedDrawerBase
    {
        /// <summary>
        /// Draws property using editor default <see cref="EditorGUILayout.PropertyField"/> method.
        /// </summary>
        /// <param name="property"></param>
        protected void DrawDefaultProperty(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property, property.isExpanded);
        }

        /// <summary>
        /// Draws property using <see cref="NestedDrawer"/> if exists or editor default <see cref="EditorGUILayout.PropertyField"/> method.
        /// </summary>
        /// <param name="property"></param>
        protected void DrawOrderedProperty(SerializedProperty property)
        {
            if (NestedDrawer != null)
            {
                NestedDrawer.HandleProperty(property);
                return;
            }

            DrawDefaultProperty(property);
        }


        /// <summary>
        /// Tries to draw provided property using custom drawer.
        /// </summary>
        /// <param name="property"></param>
        public abstract void HandleProperty(SerializedProperty property);


        /// <summary>
        /// Optional drawer. Added will create small logical hierarchy with parent and next own custom drawer.
        /// </summary>
        public OrderedDrawerBase NestedDrawer { get; set; }
    }
}