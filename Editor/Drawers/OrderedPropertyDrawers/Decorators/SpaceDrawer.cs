using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class SpaceDrawer : OrderedPropertyDrawer<OrderedSpaceAttribute>
    {
        public override void HandleProperty(SerializedProperty property, OrderedSpaceAttribute attribute)
        {
            GUILayout.Space(attribute.SpaceBefore);
            base.HandleProperty(property, attribute);
            GUILayout.Space(attribute.SpaceAfter);
        }
    }
}