using UnityEngine;

namespace Toolbox.Editor.Drawers
{ 
    public class EnableIfAttributeDrawer : ComparisonAttributeDrawer<EnableIfAttribute>
    {
        protected override PropertyCondition OnComparisonResult(bool result)
        {
            return result ? PropertyCondition.Valid : PropertyCondition.Disabled;
        }
    }
}