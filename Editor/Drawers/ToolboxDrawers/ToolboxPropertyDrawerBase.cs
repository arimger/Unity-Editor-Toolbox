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


        public virtual void OnGui(SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.PropertyField(property, label, property.isExpanded);
        }

        public virtual void OnGui(SerializedProperty property, GUIContent label, ToolboxAttribute attribute)
        {
            OnGui(property, label);    
        }

        public virtual void OnGuiReload()
        { }
    }
}