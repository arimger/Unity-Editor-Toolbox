using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public abstract class OrderedPropertyDrawerRoot
    {
        /// <summary>
        /// Draws property using <see cref="NestedDrawer"/> if exists or editor default <see cref="EditorGUILayout.PropertyField"/> method.
        /// </summary>
        /// <param name="property"></param>
        protected virtual void DrawDefaultProperty(SerializedProperty property)
        {
            if (NestedDrawer != null)
            {
                NestedDrawer.HandleProperty(property);
                return;
            }
            EditorGUILayout.PropertyField(property, property.isExpanded);
        }

        /// <summary>
        /// Draws property in custom way.
        /// </summary>
        /// <param name="property"></param>
        protected virtual void DrawCustomProperty(SerializedProperty property)
        {
            DrawDefaultProperty(property);
        }


        /// <summary>
        /// Try to draw provided property using custom drawer.
        /// </summary>
        /// <param name="property"></param>
        public abstract void HandleProperty(SerializedProperty property);


        /// <summary>
        /// Optional drawer. Added will create small logical hierarchy with parent and next own custom drawer.
        /// </summary>
        public OrderedPropertyDrawerRoot NestedDrawer { get; set; }
    }
}