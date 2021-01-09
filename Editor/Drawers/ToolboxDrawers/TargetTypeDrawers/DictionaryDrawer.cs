using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [Obsolete]
    public class DictionaryDrawer : ToolboxTargetTypeDrawer
    {
        public override void OnGui(SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.PropertyField(property, label);
        }

        public override Type GetTargetType()
        {
            return typeof(Dictionary<,>);
        }

        public override bool UseForChildren()
        {
            return false;
        }
    }
}