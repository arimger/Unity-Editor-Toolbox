using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxConditionDrawerBase : ToolboxAttributeDrawer
    {
        public abstract PropertyCondition OnGuiValidate(SerializedProperty property);

        public abstract PropertyCondition OnGuiValidate(SerializedProperty property, ToolboxAttribute attribute);
    }
}