using System;
using System.Collections.Generic;

namespace Toolbox.Editor.Drawers
{
    internal class FloatComparer : ValueComparerBase
    {
        protected override HashSet<TypeCode> GetAcceptedTypeCodes()
        {
            return new HashSet<TypeCode>()
            {
                TypeCode.Single,
                TypeCode.Double,
                TypeCode.Decimal
            };
        }


        internal override bool IsValidMethod(ValueComparisonMethod method)
        {
            return method != ValueComparisonMethod.Mask;
        }

        internal override bool Compare(object sourceValue, object targetValue, ValueComparisonMethod testMethod)
        {
            var realSourceValue = Convert.ToSingle(sourceValue);
            var realTargetValue = Convert.ToSingle(targetValue);
            switch (testMethod)
            {
                case ValueComparisonMethod.Equal:
                    return realSourceValue == realTargetValue;
                case ValueComparisonMethod.Greater:
                    return realSourceValue > realTargetValue;
                case ValueComparisonMethod.Less:
                    return realSourceValue < realTargetValue;
                case ValueComparisonMethod.GreaterEqual:
                    return realSourceValue >= realTargetValue;
                case ValueComparisonMethod.LessEqual:
                    return realSourceValue <= realTargetValue;
                default:
                    return false;
            }
        }
    }
}