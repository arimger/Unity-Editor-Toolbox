using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxPropertyDrawer<T> : ToolboxPropertyDrawerBase where T : ToolboxPropertyAttribute
    {
        public override sealed void OnGui(SerializedProperty property, GUIContent label)
        {
            var targetAttribute = property.GetAttribute<T>();
            if (targetAttribute != null)
            {
                OnGui(property, label, targetAttribute);
                return;
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            base.OnGui(property, label);
        }

        public override sealed void OnGui(SerializedProperty property, GUIContent label, ToolboxAttribute attribute)
        {
            if (attribute is T targetAttribute)
            {
                OnGui(property, label, targetAttribute);
                return;
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            base.OnGui(property, label, attribute);
        }


        public virtual void OnGui(SerializedProperty property, GUIContent label, T attribute)
        {
            base.OnGui(property, label);
        }
    }
}