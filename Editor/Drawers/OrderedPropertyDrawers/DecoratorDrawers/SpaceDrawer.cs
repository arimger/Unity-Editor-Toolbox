using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class SpaceDrawer : OrderedDecoratorDrawer<OrderedSpaceAttribute>
    {
        public SpaceDrawer() : base(null)
        { }

        public SpaceDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        { }


        protected override void OnBeforeProperty(OrderedSpaceAttribute attribute)
        {
            GUILayout.Space(attribute.SpaceBefore);
        }

        protected override void OnAfterProperty(OrderedSpaceAttribute attribute)
        {
            GUILayout.Space(attribute.SpaceAfter);
        }
    }
}