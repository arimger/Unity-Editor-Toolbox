using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class ToolboxConditionDrawer<T> : ToolboxConditionDrawerBase where T : ToolboxConditionAttribute
    {
        public override sealed PropertyCondition OnGuiValidate(SerializedProperty property)
        {
            var targetAttribute = property.GetAttribute<T>();
            if (targetAttribute != null)
            {
                return OnGuiValidate(property, targetAttribute);
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            return base.OnGuiValidate(property);
        }

        public override sealed PropertyCondition OnGuiValidate(SerializedProperty property, ToolboxAttribute attribute)
        {
            if (attribute is T targetAttribute)
            {
                return OnGuiValidate(property, targetAttribute);
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            return base.OnGuiValidate(property, attribute);
        }


        public virtual PropertyCondition OnGuiValidate(SerializedProperty property, T attribute)
        {
            return base.OnGuiValidate(property, attribute);
        }
    }
}