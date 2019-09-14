using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class HideAttributeDrawer : ToolboxConditionDrawer<HideAttribute>
    {
        public override PropertyCondition OnGuiValidate(SerializedProperty property, HideAttribute attribute)
        {
            return PropertyCondition.NonValid;
        }
    }
}