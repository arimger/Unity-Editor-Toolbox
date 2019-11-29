using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class DictionaryDrawer : ToolboxTargetTypeDrawer
    {
        public override void OnGui(SerializedProperty property, GUIContent label)
        {
            //TODO
            EditorGUILayout.PropertyField(property, label);
        }

        public override Type GetTargetType()
        {
            return typeof(SerializedDictionary<,>);
        }
    }
}