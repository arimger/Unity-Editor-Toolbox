using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxTargetTypeDrawer : ToolboxDrawer
    {
        public abstract void OnGui(SerializedProperty property, GUIContent label);

        public abstract Type GetTargetType();
        public abstract bool UseForChildren();
    }
}