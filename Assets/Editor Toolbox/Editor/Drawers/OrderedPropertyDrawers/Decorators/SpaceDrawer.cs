using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class SpaceDrawer : OrderedPropertyDrawer<ToolboxSpaceAttribute>
    {
        public override void HandleProperty(SerializedProperty property, ToolboxSpaceAttribute attribute)
        {
            GUILayout.Space(attribute.SpaceBefore);
            base.HandleProperty(property, attribute);
            GUILayout.Space(attribute.SpaceAfter);
        }
    }
}