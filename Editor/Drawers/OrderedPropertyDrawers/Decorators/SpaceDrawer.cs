using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class SpaceDrawer : OrderedPropertyDrawer<OrderedSpaceAttribute>
    {
        public override void HandleTargetProperty(SerializedProperty property, OrderedSpaceAttribute attribute)
        {
            GUILayout.Space(attribute.SpaceBefore);
            base.HandleTargetProperty(property, attribute);
            GUILayout.Space(attribute.SpaceAfter);
        }
    }
}