using System;
using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace Toolbox.Comparison
{
    internal class ObjectComparer : ValueComparerBase
    {
        protected override HashSet<TypeCode> GetAcceptedTypeCodes()
        {
            return new HashSet<TypeCode>()
            {
                TypeCode.Object
            };
        }


        internal override bool IsValidSource(object value)
        {
            return value is Object && base.IsValidSource(value);
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
            switch (method)
            {
                case ValueComparisonMethod.Equal:
                    return (Object)sourceValue == (bool)targetValue;
                default:
                    return false;
            }
        }
    }
}