using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxPropertyDrawerBase : ToolboxAttributeDrawer
    {
        public abstract bool IsPropertyValid(SerializedProperty property);

        public abstract void OnGui(SerializedProperty property, GUIContent label);

        public abstract void OnGui(SerializedProperty property, GUIContent label, ToolboxAttribute attribute);

        public virtual void OnGuiReload()
        { }
    }
}