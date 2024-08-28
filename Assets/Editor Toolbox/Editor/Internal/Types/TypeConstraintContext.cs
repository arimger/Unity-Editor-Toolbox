using System;
using System.Collections.Generic;

namespace Toolbox.Editor.Internal.Types
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
            return type.IsVisible;
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