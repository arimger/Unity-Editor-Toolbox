using System;

namespace Toolbox.Editor.Drawers
{
    [Obsolete]
    public struct ValueComparisonInput
    {
        public object sourceValue;
        public object targetValue;
        public ValueComparisonMethod method;
    }
}