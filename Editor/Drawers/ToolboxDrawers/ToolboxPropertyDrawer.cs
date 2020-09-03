using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxPropertyDrawer<T> : ToolboxPropertyDrawerBase where T : ToolboxPropertyAttribute
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
            OnGui(property, label, PropertyUtility.GetAttribute<T>(property));
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
                ToolboxEditorLog.WrongAttributeUsageWarning(attribute, property);
                ToolboxEditorGui.DrawLayoutEmptyProperty(property, warningContent);
            }
        }
    }
}