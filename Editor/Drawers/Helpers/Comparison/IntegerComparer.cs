using System;
using System.Collections.Generic;

namespace Toolbox.Editor.Drawers
{
    internal class IntegerComparer : ValueComparerBase
    {
        protected override HashSet<TypeCode> GetAcceptedTypeCodes()
        {
            return new HashSet<TypeCode>()
            {
                TypeCode.SByte,
                TypeCode.Byte,
                TypeCode.Int16,
                TypeCode.UInt16,
                TypeCode.Int32,
                TypeCode.UInt32,
                TypeCode.Int64,
                TypeCode.UInt64
            };
        }


        internal override bool Compare(object sourceValue, object targetValue, ValueComparisonMethod method)
        {
            var realSourceValue = Convert.ToInt32(sourceValue);
            var realTargetValue = Convert.ToInt32(targetValue);
            switch (method)
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
                case ValueComparisonMethod.Mask:
                    return (realSourceValue & realTargetValue) == realTargetValue;
                default:
                    return false;
            }
        }
    }
}