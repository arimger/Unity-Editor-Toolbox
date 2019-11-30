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

    public abstract class ToolboxConditionDrawerBase : ToolboxAttributeDrawer
    {
        public abstract PropertyCondition OnGuiValidate(SerializedProperty property);

        public abstract PropertyCondition OnGuiValidate(SerializedProperty property, ToolboxAttribute attribute);
    }
}