using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class DisableAttributeDrawer : ToolboxConditionDrawer<DisableAttribute>
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, DisableAttribute attribute)
        {
            return PropertyCondition.Disabled;
        }
    }
}