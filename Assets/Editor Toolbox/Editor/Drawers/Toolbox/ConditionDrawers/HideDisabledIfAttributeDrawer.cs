using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class HideDisabledIfAttributeDrawer : ComparisonAttributeDrawer<HideDisabledIfAttribute>
    {
        protected override PropertyCondition OnComparisonResult(bool result)
        {
            return result ? PropertyCondition.NonValid : PropertyCondition.Disabled;
        }
    }
}