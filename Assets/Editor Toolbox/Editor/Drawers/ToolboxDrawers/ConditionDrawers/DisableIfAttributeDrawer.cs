using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class DisableIfAttributeDrawer : ComparisonAttributeDrawer<DisableIfAttribute>
    {
        protected override PropertyCondition OnComparisonResult(bool result)
        {
            return result ? PropertyCondition.Disabled : PropertyCondition.Valid;
        }
    }
}