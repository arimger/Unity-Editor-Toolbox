using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class HideIfAttributeDrawer : ConditionAttributeDrawer<HideIfAttribute>
    {
        public override PropertyCondition OnGuiValidate(SerializedProperty property, HideIfAttribute attribute)
        {
            return IsConditionMet(property, attribute) ? PropertyCondition.Valid : PropertyCondition.NonValid;
        }
    }
}