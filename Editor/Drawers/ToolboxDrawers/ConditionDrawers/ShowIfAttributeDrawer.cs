using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class ShowIfAttributeDrawer : ComparisonAttributeDrawer<ShowIfAttribute>
    {
        protected override PropertyCondition OnComparisonResult(bool result)
        {
            return result ? PropertyCondition.Valid : PropertyCondition.NonValid;
        }
    }
}