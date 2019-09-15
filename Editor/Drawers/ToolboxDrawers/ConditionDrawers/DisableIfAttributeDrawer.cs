using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class DisableIfAttributeDrawer : ConditionAttributeDrawer<DisableIfAttribute>
    {
        public override PropertyCondition OnGuiValidate(SerializedProperty property, DisableIfAttribute attribute)
        {
            return IsConditionMet(property, attribute) ? PropertyCondition.Valid : PropertyCondition.Disabled;
        }
    }
}