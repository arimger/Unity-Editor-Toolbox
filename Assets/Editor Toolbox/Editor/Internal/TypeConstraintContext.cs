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
            return type.IsVisible;
        }

        public virtual void ApplyTarget(Type type)
        {
            targetType = type;
        }

        public override bool Equals(object obj)
        {
            return obj is TypeConstraintContext constraint &&
                   EqualityComparer<Type>.Default.Equals(targetType, constraint.targetType);
        }

        public override int GetHashCode()
        {
            return 1673078848 + EqualityComparer<Type>.Default.GetHashCode(targetType);
        }


        public Type TargetType => targetType;
    }
}