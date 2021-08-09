using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxConditionDrawerBase : ToolboxAttributeDrawer
    {
        public abstract PropertyCondition OnGuiValidate(SerializedProperty property);

        public abstract PropertyCondition OnGuiValidate(SerializedProperty property, ToolboxAttribute attribute);
    }
}