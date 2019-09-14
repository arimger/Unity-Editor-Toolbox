using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public enum PropertyCondition
    {
        Valid,
        NonValid,
        Disabled
    }

    public abstract class ToolboxConditionDrawerBase : ToolboxDrawer
    {
        public virtual PropertyCondition OnGuiValidate(SerializedProperty property)
        {
            return PropertyCondition.Valid;
        }

        public virtual PropertyCondition OnGuiValidate(SerializedProperty property, ToolboxAttribute attribute)
        {
            return OnGuiValidate(property);
        }
    }
}