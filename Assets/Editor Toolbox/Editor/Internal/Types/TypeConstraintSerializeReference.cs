using System;
using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace Toolbox.Editor.Internal.Types
{
    /// <summary>
    /// Dedicated <see cref="TypeConstraintContext"/> for SerializeReference-based types.
    /// </summary>
    public class TypeConstraintSerializeReference : TypeConstraintContext
    {
        public TypeConstraintSerializeReference(Type targetType) : base(targetType)
        { }

        public override bool IsSatisfied(Type type)
        {
            //NOTE: generic types are not supported below Unity 2023 while using the Serialize References
#if !UNITY_2023_2_OR_NEWER
            if (type.IsGenericType)
            {
                return false;
            }
#endif
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
            return other is TypeConstraintSerializeReference constraint &&
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