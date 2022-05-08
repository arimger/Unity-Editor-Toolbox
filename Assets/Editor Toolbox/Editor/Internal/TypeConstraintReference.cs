using System;
using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace Toolbox.Editor.Internal
{
    public class TypeConstraintReference : TypeConstraintContext
    {
        public TypeConstraintReference(Type targetType) : base(targetType)
        { }


        public override bool IsSatisfied(Type type)
        {
            return base.IsSatisfied(type) &&
                !type.IsInterface &&
                !type.IsAbstract &&
                !type.IsPointer &&
                !type.IsArray &&
                !type.IsSubclassOf(typeof(Object)) &&
                !type.ContainsGenericParameters &&
                !Attribute.IsDefined(type, typeof(ObsoleteAttribute));
        }

        public override bool Equals(object other)
        {
            return other is TypeConstraintReference constraint &&
                   base.Equals(other) &&
                   EqualityComparer<Type>.Default.Equals(targetType, constraint.targetType);
        }

        public override int GetHashCode()
        {
            var hashCode = 1038385366;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            return hashCode;
        }
    }
}