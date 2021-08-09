using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class ShowDisabledIfAttributeDrawer : ComparisonAttributeDrawer<ShowDisabledIfAttribute>
    {
        protected override PropertyCondition OnComparisonResult(bool result)
        {
            return result ? PropertyCondition.Disabled : PropertyCondition.NonValid;
        }
    }
}