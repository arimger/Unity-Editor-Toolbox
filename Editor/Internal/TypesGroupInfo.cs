using System;
using System.Collections.Generic;

using UnityEngine;

namespace Toolbox.Editor.Internal
{
    //TODO: refactor
    public class TypesGroupInfo
    {
        private readonly string[] options;
        private readonly List<Type> types;
        private readonly TypeGrouping grouping;
        private readonly TypeConstraint constraint;
        private readonly bool addEmptyValue;


        public TypesGroupInfo(TypeConstraint constraint, List<Type> types, bool addEmptyValue = true, TypeGrouping grouping = TypeGrouping.None)
        {
            this.constraint = constraint;
            this.types = types;
            this.addEmptyValue = addEmptyValue;
            this.grouping = grouping;

            var count = types.Count;
            var shift = 0;
            if (addEmptyValue)
            {
                shift = 1;
                count += 1;
                options = new string[count];
                options[0] = "<none>";
            }
            else
            {
                options = new string[count];
            }

            
            for (var i = 0; i < count - shift; i++)
            {
                var type = types[i];
                var name = FormatGroupedTypeName(type, grouping);
                options[i + shift] = name;
            }
        }


        private static string FormatGroupedTypeName(Type type, TypeGrouping grouping)
        {
            var name = type.FullName;
            switch (grouping)
            {
                default:
                case TypeGrouping.None:
                    return name;

                case TypeGrouping.ByNamespace:
                    return name.Replace('.', '/');

                case TypeGrouping.ByNamespaceFlat:
                    var lastPeriodIndex = name.LastIndexOf('.');
                    if (lastPeriodIndex != -1)
                    {
                        name = name.Substring(0, lastPeriodIndex) + "/" + name.Substring(lastPeriodIndex + 1);
                    }

                    return name;

                case TypeGrouping.ByAddComponentMenu:
                    var addComponentMenuAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
                    if (addComponentMenuAttributes.Length == 1)
                    {
                        return ((AddComponentMenu)addComponentMenuAttributes[0]).componentMenu;
                    }

                    return "Scripts/" + type.FullName.Replace('.', '/');
            }
        }


        public int IndexOf(Type type)
        {
            var index = -1;
            if (type != null)
            {
                index = types.IndexOf(type);
            }

            return addEmptyValue
                ? index + 1
                : index;
        }


        public string[] Options => options;
        public IReadOnlyList<Type> Types => types;
    }
}