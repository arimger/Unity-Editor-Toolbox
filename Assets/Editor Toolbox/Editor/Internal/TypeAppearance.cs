using System.Collections.Generic;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    //TODO: refactor
    public class TypeAppearance
    {
        public TypeAppearance(TypeConstraint constraint) : this(constraint, TypeGrouping.None, true)
        { }

        public TypeAppearance(TypeConstraint constraint, TypeGrouping grouping, bool addEmptyValue)
        {
            Constraint = constraint;
            Grouping = grouping;
            AddEmptyValue = addEmptyValue;
        }


        public override bool Equals(object other)
        {
            return other is TypeAppearance settings &&
                   Grouping == settings.Grouping &&
                   EqualityComparer<TypeConstraint>.Default.Equals(Constraint, settings.Constraint) &&
                   AddEmptyValue == settings.AddEmptyValue;
        }

        public override int GetHashCode()
        {
            var hashCode = -8527728;
            hashCode = hashCode * -1521134295 + Grouping.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TypeConstraint>.Default.GetHashCode(Constraint);
            hashCode = hashCode * -1521134295 + AddEmptyValue.GetHashCode();
            return hashCode;
        }


        public TypeConstraint Constraint { get; set; }
        public TypeGrouping Grouping { get; set; }
        public bool AddEmptyValue { get; set; }
    }
}