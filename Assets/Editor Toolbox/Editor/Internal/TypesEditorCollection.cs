using System;

using UnityEngine;

namespace Toolbox.Editor.Internal
{
    public class TypesEditorCollection : TypesCachedCollection
    {
        private readonly bool hasEmptyValue;
        private string[] labels;


        public TypesEditorCollection(TypesCachedCollection cachedCollection)
            : this(cachedCollection, true)
        { }

        public TypesEditorCollection(TypesCachedCollection cachedCollection, bool hasEmptyValue)
            : this(cachedCollection, hasEmptyValue, TypeGrouping.None)
        { }

        public TypesEditorCollection(TypesCachedCollection cachedCollection, bool hasEmptyValue, TypeGrouping grouping)
            : base(cachedCollection)
        {
            this.hasEmptyValue = hasEmptyValue;
            CreateLabels(grouping);
        }


        private void CreateLabels(TypeGrouping grouping)
        {
            var count = Values.Count;
            var shift = 0;
            if (hasEmptyValue)
            {
                shift += 1;
                count += 1;
                labels = new string[count];
                labels[0] = "<none>";
            }
            else
            {
                labels = new string[count];
            }

            for (var i = 0; i < count - shift; i++)
            {
                var type = Values[i];
                var name = FormatGroupedTypeName(type, grouping);
                labels[i + shift] = name;
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

                case TypeGrouping.ByFlatName:
                    return type.Name;
            }
        }


        public override int IndexOf(Type type)
        {
            var index = -1;
            if (type != null)
            {
                index = base.IndexOf(type);
            }

            return hasEmptyValue
                ? index + 1
                : index;
        }


        public string[] Labels => labels;
    }
}