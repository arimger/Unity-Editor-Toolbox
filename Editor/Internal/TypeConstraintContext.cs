using System;
using System.Collections.Generic;

namespace Toolbox.Editor.Internal
{
    public class TypeConstraintContext
    {
        protected Type targetType;


        public TypeConstraintContext(Type targetType)
        {
            this.targetType = targetType;
        }


        public virtual bool IsSatisfied(Type type)
        {
#if UNITY_2019_2_OR_NEWER
            return type.IsVisible;
#else
            return type.IsVisible && (targetType.IsGenericType
                ? targetType.IsAssignableFromGeneric(type)
                : targetType.IsAssignableFrom(type));
#endif
        }

        public virtual void ApplyTarget(Type type)
        {
            targetType = type;
        }

        public override bool Equals(object obj)
        {
            return obj is TypeConstraintContext context &&
                   EqualityComparer<Type>.Default.Equals(TargetType, context.TargetType) &&
                   IsOrdered == context.IsOrdered;
        }

        public override int GetHashCode()
        {
            var hashCode = -509589530;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(TargetType);
            hashCode = hashCode * -1521134295 + IsOrdered.GetHashCode();
            return hashCode;
        }


        public Type TargetType => targetType;
        public Comparison<Type> Comparer { get; set; } = (t1, t2) => t1.Name.CompareTo(t2.Name);
        public bool IsOrdered { get; set; } = true;
    }
}