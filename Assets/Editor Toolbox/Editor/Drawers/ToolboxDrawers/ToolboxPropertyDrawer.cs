using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxPropertyDrawer<T> : ToolboxPropertyDrawerBase where T : ToolboxPropertyAttribute
    {
        public override sealed void OnGui(SerializedProperty property)
        {
            var targetAttribute = property.GetAttribute<T>();
            if (targetAttribute != null)
            {
                OnGui(property, targetAttribute);
                return;
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            base.OnGui(property);
        }

        public override sealed void OnGui(SerializedProperty property, ToolboxAttribute attribute)
        {
            if (attribute is T targetAttribute)
            {
                OnGui(property, targetAttribute);
                return;
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            base.OnGui(property, attribute);
        }


        public virtual void OnGui(SerializedProperty property, T attribute)
        {
            base.OnGui(property);
        }
    }
}