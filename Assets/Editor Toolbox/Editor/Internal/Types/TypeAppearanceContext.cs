using System.Collections.Generic;

using UnityEngine;

namespace Toolbox.Editor.Internal.Types
{
    public class TypeAppearanceContext
    {
        public TypeAppearanceContext(TypeConstraintContext constraint) : this(constraint, TypeGrouping.None, true)
        { }

        public TypeAppearanceContext(TypeConstraintContext constraint, TypeGrouping typeGrouping, bool addEmptyValue)
        {
            Constraint = constraint;
            TypeGrouping = typeGrouping;
            AddEmptyValue = addEmptyValue;
        }


        public override bool Equals(object other)
        {
            return other is TypeAppearanceContext appearance &&
                    EqualityComparer<TypeConstraintContext>.Default.Equals(Constraint, appearance.Constraint) &&
                    TypeGrouping == appearance.TypeGrouping &&
                    AddEmptyValue == appearance.AddEmptyValue;
        }

        public override int GetHashCode()
        {
            var hashCode = -8527728;
            hashCode = hashCode * -1521134295 + EqualityComparer<TypeConstraintContext>.Default.GetHashCode(Constraint);
            hashCode = hashCode * -1521134295 + TypeGrouping.GetHashCode();
            hashCode = hashCode * -1521134295 + AddEmptyValue.GetHashCode();
            return hashCode;
        }


        public TypeConstraintContext Constraint { get; set; }
        public TypeGrouping TypeGrouping { get; set; }
        public bool AddEmptyValue { get; set; }
    }
}