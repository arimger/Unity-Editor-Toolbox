using System;

using UnityEngine;

namespace Toolbox.Editor.Internal.Types
{
    public class TypeConstraintStandard : TypeConstraintContext
    {
        public TypeConstraintStandard() : base(null)
        { }

        public TypeConstraintStandard(Type targetType, TypeSettings settings, bool allowAbstract, bool allowObsolete) : base(targetType)
        {
            Settings = settings;
            AllowAbstract = allowAbstract;
            AllowObsolete = allowObsolete;
        }

        public override bool IsSatisfied(Type type)
        {
            return base.IsSatisfied(type) &&
                //NOTE: consider moving allowAbstract && allowObsolete properties to the TypeSettings enum
                (!type.IsClass || Settings.HasFlag(TypeSettings.Class)) &&
                (!type.IsAbstract || (Settings.HasFlag(TypeSettings.Interface) && type.IsInterface) || AllowAbstract) &&
                (!type.IsInterface || Settings.HasFlag(TypeSettings.Interface)) &&
                (!Attribute.IsDefined(type, typeof(ObsoleteAttribute)) || AllowObsolete);
        }

        public override bool Equals(object other)
        {
            return other is TypeConstraintStandard constraint &&
                   base.Equals(other) &&
                   Settings == constraint.Settings &&
                   AllowAbstract == constraint.AllowAbstract &&
                   AllowObsolete == constraint.AllowObsolete;
        }

        public override int GetHashCode()
        {
            var hashCode = 433750135;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Settings.GetHashCode();
            hashCode = hashCode * -1521134295 + AllowAbstract.GetHashCode();
            hashCode = hashCode * -1521134295 + AllowObsolete.GetHashCode();
            return hashCode;
        }

        public TypeSettings Settings { get; set; }
        public bool AllowAbstract { get; set; }
        public bool AllowObsolete { get; set; }
    }
}