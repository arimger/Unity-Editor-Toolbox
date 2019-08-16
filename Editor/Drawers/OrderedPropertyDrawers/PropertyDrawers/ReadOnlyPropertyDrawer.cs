using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class ReadOnlyPropertyDrawer : OrderedPropertyDrawer<ReadOnlyAttribute>
    {
        public ReadOnlyPropertyDrawer() : base(null)
        { }

        public ReadOnlyPropertyDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        { }


        /// <summary>
        /// Drawer method handled by ancestor class.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        /// <param name="attribute"></param>
        public override void HandleTargetProperty(SerializedProperty property, ReadOnlyAttribute attribute)
        {
            EditorGUI.BeginDisabledGroup(true);
            base.HandleTargetProperty(property, attribute);
            EditorGUI.EndDisabledGroup();
        }
    }
}