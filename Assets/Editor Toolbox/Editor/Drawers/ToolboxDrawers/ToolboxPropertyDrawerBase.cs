using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxPropertyDrawerBase : ToolboxAttributeDrawer
    {
        public ToolboxPropertyDrawerBase()
        {
            ToolboxDrawerUtility.onEditorReload += OnGuiReload;
        }


        public abstract bool IsPropertyValid(SerializedProperty property);

        public abstract void OnGui(SerializedProperty property, GUIContent label);

        public abstract void OnGui(SerializedProperty property, GUIContent label, ToolboxAttribute attribute);
 

        public virtual void OnGuiReload()
        { }
    }

    public abstract class ToolboxPropertyDrawerBase<T> : ToolboxPropertyDrawerBase where T : ToolboxAttribute
    {
        protected virtual void OnGuiSafe(SerializedProperty property, GUIContent label, T attribute)
        {
            ToolboxEditorGui.DrawLayoutDefaultProperty(property);
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return true;
        }

        public override sealed void OnGui(SerializedProperty property, GUIContent label)
        {
            OnGui(property, label, property.GetAttribute<T>());
        }

        public override sealed void OnGui(SerializedProperty property, GUIContent label, ToolboxAttribute attribute)
        {
            OnGui(property, label, attribute as T);
        }


        public void OnGui(SerializedProperty property, GUIContent label, T attribute)
        {
            if (attribute == null)
            {
                return;
            }

            if (IsPropertyValid(property))
            {
                OnGuiSafe(property, label, attribute);
            }
            else
            {
                var warningContent = new GUIContent(property.displayName + " has invalid property drawer");
                ToolboxEditorLog.WrongAttributeUsageWarning(property, attribute);
                ToolboxEditorGui.DrawLayoutEmptyProperty(property, warningContent);
            }
        }
    }
}