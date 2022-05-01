using System;

namespace Toolbox.Editor.Internal
{
    public class TypeConstraintStandard : TypeConstraint
    {
        private readonly bool allowAbstract;
        private readonly bool allowObsolete;


        public TypeConstraintStandard(Type targetType, bool allowAbstract, bool allowObsolete) : base(targetType)
        {
            this.allowAbstract = allowAbstract;
            this.allowObsolete = allowObsolete;
        }


        public override bool IsSatisfied(Type type)
        {
            if (!base.IsSatisfied(type))
            {
                return false;
            }

            return (allowAbstract || !type.IsAbstract) && (allowObsolete || !Attribute.IsDefined(type, typeof(ObsoleteAttribute)));
        }

        public override bool Equals(object other)
        {
            return other is TypeConstraintStandard constraint &&
                   base.Equals(other) &&
                   allowAbstract == constraint.allowAbstract &&
                   allowObsolete == constraint.allowObsolete;
        }

        public override int GetHashCode()
        {
            var hashCode = 433750135;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + allowAbstract.GetHashCode();
            hashCode = hashCode * -1521134295 + allowObsolete.GetHashCode();
            return hashCode;
        }
    }
}