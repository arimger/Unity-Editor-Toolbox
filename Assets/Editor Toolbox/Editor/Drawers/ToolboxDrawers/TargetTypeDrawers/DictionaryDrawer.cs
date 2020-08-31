using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [Obsolete]
    public class DictionaryDrawer : ToolboxTargetTypeDrawer
    {
        public override void OnGui(SerializedProperty property, GUIContent label)
        {
            //TODO:
            EditorGUILayout.PropertyField(property, label);
        }

        public override Type GetTargetType()
        {
            return typeof(SerializedDictionary<,>);
        }

        public override bool UseForChildren()
        {
            return false;
        }
    }
}