using System.Collections.Generic;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    public class TypesGroupSettings
    {
        public TypesGroupSettings(ClassGrouping grouping, TypeConstraint constraint, bool addEmptyValue)
        {
            Grouping = grouping;
            Constraint = constraint;
            AddEmptyValue = addEmptyValue;
        }


        public override bool Equals(object other)
        {
            return other is TypesGroupSettings settings &&
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


        public ClassGrouping Grouping { get; set; }
        public TypeConstraint Constraint { get; set; }
        public bool AddEmptyValue { get; set; }
    }
}