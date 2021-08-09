using System;
using System.Collections.Generic;

namespace Toolbox.Editor.Drawers
{
    internal class StringComparer : ValueComparerBase
    {
        protected override HashSet<TypeCode> GetAcceptedTypeCodes()
        {
            return new HashSet<TypeCode>()
            {
                TypeCode.Char,
                TypeCode.String
            };
        }


        internal override bool IsValidMethod(ValueComparisonMethod method)
        {
            return method == ValueComparisonMethod.Equal;
        }

        internal override bool Compare(object sourceValue, object targetValue, ValueComparisonMethod method)
        {
            switch (method)
            {
                case ValueComparisonMethod.Equal:
                    return sourceValue.Equals(targetValue);
                default:
                    return false;
            }
        }
    }
}