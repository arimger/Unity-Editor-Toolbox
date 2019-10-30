using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxPropertyDrawerBase : ToolboxDrawer
    {
        public ToolboxPropertyDrawerBase()
        {
            ToolboxDrawerUtility.onEditorReload += OnGuiReload;
        }


        public virtual void OnGui(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property, property.isExpanded);
        }

        public virtual void OnGui(SerializedProperty property, ToolboxAttribute attribute)
        {
            OnGui(property);    
        }

        public virtual void OnGuiReload()
        { }
    }
}