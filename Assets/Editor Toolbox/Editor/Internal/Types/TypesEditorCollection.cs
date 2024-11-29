using System;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Toolbox.Editor.Internal.Types
{
    /// <summary>
    /// Dedicated collection that additionally holds labels linked to associated <see cref="Type"/>s.
    /// Useful for Editor-based drawers and tools.
    /// </summary>
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

        /// <summary>
        /// Creates a bit nicer (in terms of display) <see cref="Type.FullName"/> or <see cref="Type.Name"/> equivalent.
        /// </summary>
        private static string GetTypeName(Type type, bool createFull)
        {
            var stringBuilder = new StringBuilder();
            if (createFull)
            {
                var namespaceName = type.Namespace;
                if (!string.IsNullOrEmpty(namespaceName))
                {
                    stringBuilder.Append(type.Namespace);
                    stringBuilder.Append('.');
                }
            }

            if (type.IsNested)
            {
                var declaringType = type.DeclaringType;
                stringBuilder.Append(declaringType.Name);
                stringBuilder.Append(':');
            }

            var typeName = type.Name;
            //NOTE: there are rare cases where "generic" types have no arguments, let's ignore them
            if (type.IsGenericType && typeName.Contains("`"))
            {
                var genericCharIndex = typeName.IndexOf("`");
                typeName = typeName.Substring(0, genericCharIndex);

                stringBuilder.Append(typeName);
                stringBuilder.Append('<');
                var arguments = type.GetGenericArguments();
                for (var i = 0; i < arguments.Length; i++)
                {
                    var argumentType = arguments[i];
                    var argumentName = string.IsNullOrEmpty(argumentType.FullName)
                        ? argumentType.Name
                        : GetTypeName(argumentType, false);

                    stringBuilder.Append(argumentName);
                    if (i < arguments.Length - 1)
                    {
                        stringBuilder.Append(", ");
                    }
                }

                stringBuilder.Append('>');
            }
            else
            {
                stringBuilder.Append(typeName);
            }

            return stringBuilder.ToString();
        }

        private static string FormatGroupedTypeName(Type type, TypeGrouping grouping)
        {
            switch (grouping)
            {
                default:
                case TypeGrouping.None:
                    {
                        var name = GetTypeName(type, true);
                        return name;
                    }
                case TypeGrouping.ByNamespace:
                    {
                        var name = GetTypeName(type, true);
                        return name.Replace('.', '/');
                    }
                case TypeGrouping.ByNamespaceFlat:
                    {
                        var name = GetTypeName(type, true);
                        var lastPeriodIndex = name.LastIndexOf('.');
                        if (lastPeriodIndex != -1)
                        {
                            name = name.Substring(0, lastPeriodIndex) + "/" + name.Substring(lastPeriodIndex + 1);
                        }

                        return name;
                    }
                case TypeGrouping.ByAddComponentMenu:
                    {
                        var addComponentMenuAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
                        if (addComponentMenuAttributes.Length == 1)
                        {
                            return ((AddComponentMenu)addComponentMenuAttributes[0]).componentMenu;
                        }

                        return "Scripts/" + type.FullName.Replace('.', '/');
                    }
                case TypeGrouping.ByFlatName:
                    {
                        var name = GetTypeName(type, false);
                        return name;
                    }
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