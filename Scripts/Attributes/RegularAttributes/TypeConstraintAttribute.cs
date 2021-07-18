using System;
using System.Collections.Generic;
using System.Reflection;

using Toolbox;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick proper type using popup control.
    /// 
    /// <para>Supported types: <see cref="SerializedType"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class TypeConstraintAttribute : PropertyAttribute
    {
        protected TypeConstraintAttribute(Type assemblyType)
        {
            AssemblyType = assemblyType;
        }


        /// <summary>
        /// Get all proper types from executing assembly.
        /// </summary>
        public List<Type> GetFilteredTypes()
        {
            var types = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                types.AddRange(GetFilteredTypes(assembly));
            }

            types.Sort((a, b) => a.FullName.CompareTo(b.FullName));
            return types;
        }

        /// <summary>
        /// Get all filtered type from provided assembly.
        /// </summary>
        public List<Type> GetFilteredTypes(Assembly assembly)
        {
            var types = new List<Type>();
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsVisible || !type.IsClass)
                {
                    continue;
                }

                if (!IsConstraintSatisfied(type))
                {
                    continue;
                }

                types.Add(type);
            }

            return types;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Type"/> satisfies filter constraint.
        /// </summary>
        /// <param name="type">Type to test.</param>
        /// <returns>
        /// A <see cref="bool"/> value indicating if the type specified by <paramref name="type"/>
        /// satisfies this constraint and should thus be selectable.
        /// </returns>
        public virtual bool IsConstraintSatisfied(Type type)
        {
            return (AllowAbstract || !type.IsAbstract) && (AllowObsolete || !IsDefined(type, typeof(ObsoleteAttribute)));
        }

        ///<inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ AssemblyType.GetHashCode();
                result = (result * 397) ^ AllowAbstract.GetHashCode();
                result = (result * 397) ^ AllowObsolete.GetHashCode();
                return result;
            }
        }


        /// <summary>
        /// Associated type which will define what type we are looking for.
        /// </summary>
        public Type AssemblyType { get; private set; }

        /// <summary>
        /// Gets or sets whether abstract classes can be selected from drop-down.
        /// Defaults to a value of <c>false</c> unless explicitly specified.
        /// </summary>
        public bool AllowAbstract { get; set; }
        /// <summary>
        /// Gets or sets whether obsolete classes can be selected from drop-down.
        /// Defaults to a value of <c>false</c> unless explicitly specified.
        /// </summary>
        public bool AllowObsolete { get; set; }

        /// <summary>
        /// Indicates if created popup menu should have an additional search field.
        /// </summary>
        public bool AddTextSearchField { get; set; }

        /// <summary>
        /// Gets or sets grouping of selectable classes.
        /// Defaults to <see cref="ClassGrouping.None"/> unless explicitly specified.
        /// </summary>
        public ClassGrouping Grouping { get; set; } = ClassGrouping.None;
    }

    ///<inheritdoc/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ClassExtendsAttribute : TypeConstraintAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExtendsAttribute"/> class.
        /// </summary>
        public ClassExtendsAttribute() : base(typeof(object))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExtendsAttribute"/> class.
        /// </summary>
        /// <param name="baseType">Type of class that selectable classes must derive from.</param>
        public ClassExtendsAttribute(Type baseType) : base(baseType)
        { }


        public override bool IsConstraintSatisfied(Type type)
        {
            if (type == AssemblyType || !base.IsConstraintSatisfied(type))
            {
                return false;
            }

            return AssemblyType.IsGenericType
                ? AssemblyType.IsAssignableFromGeneric(type)
                : AssemblyType.IsAssignableFrom(type);
        }
    }

    ///<inheritdoc/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ClassImplementsAttribute : TypeConstraintAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassImplementsAttribute"/> class.
        /// </summary>
        public ClassImplementsAttribute() : base(null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassImplementsAttribute"/> class.
        /// </summary>
        /// <param name="interfaceType">Type of interface that selectable classes must implement.</param>
        public ClassImplementsAttribute(Type interfaceType) : base(interfaceType)
        { }


        public override bool IsConstraintSatisfied(Type type)
        {
            if (base.IsConstraintSatisfied(type))
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (interfaceType == AssemblyType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Indicates how selectable classes should be collated in drop-down menu.
    /// </summary>
    public enum ClassGrouping
    {
        /// <summary>
        /// No grouping, just show type names in a list; for instance, "Some.Nested.Namespace.SpecialClass".
        /// </summary>
        None,
        /// <summary>
        /// Group classes by namespace and show foldout menus for nested namespaces; for
        /// instance, "Some > Nested > Namespace > SpecialClass".
        /// </summary>
        ByNamespace,
        /// <summary>
        /// Group classes by namespace; for instance, "Some.Nested.Namespace > SpecialClass".
        /// </summary>
        ByNamespaceFlat,
        /// <summary>
        /// Group classes in the same way as Unity does for its component menu. This
        /// grouping method must only be used for <see cref="MonoBehaviour"/> types.
        /// </summary>
        ByAddComponentMenu,
    }
}