using System;
using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace Toolbox.Editor.Drawers
{
    internal class ObjectComparer : ValueComparerBase
    {
        protected override HashSet<TypeCode> GetAcceptedTypeCodes()
        {
            return new HashSet<TypeCode>()
            {
                TypeCode.Empty,
                TypeCode.Object
            };
        }


        internal override bool IsValidSource(object value)
        {
            return base.IsValidSource(value);
        }

        internal override bool IsValidTarget(object value)
        {
            return value is bool;
        }

        internal override bool IsValidMethod(ValueComparisonMethod method)
        {
            return method == ValueComparisonMethod.Equal;
        }

        internal override bool Compare(object sourceValue, object targetValue, ValueComparisonMethod method)
        {
            var realTargetValue = (bool)targetValue;
            switch (method)
            {
                case ValueComparisonMethod.Equal:
                    return sourceValue != null
                        ? realTargetValue == (Object)sourceValue
                        : realTargetValue == false;
                default:
                    return false;
            }
        }
    }
}