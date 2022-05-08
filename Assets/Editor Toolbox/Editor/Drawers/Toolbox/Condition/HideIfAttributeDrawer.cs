using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class HideIfAttributeDrawer : ComparisonAttributeDrawer<HideIfAttribute>
    {
        protected override PropertyCondition OnComparisonResult(bool result)
        {
            return result ? PropertyCondition.NonValid : PropertyCondition.Valid;
        }
    }
}