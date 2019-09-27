using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class DisableAttributeDrawer : ToolboxConditionDrawer<DisableAttribute>
    {
        public override PropertyCondition OnGuiValidate(SerializedProperty property, DisableAttribute attribute)
        {
            return PropertyCondition.Disabled;
        }
    }
}