using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine
{
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

    /// <summary>
    /// Base class for class selection constraints that can be applied when selecting
    /// a <see cref="SerializedType"/> with the Unity inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class ClassTypeConstraintAttribute : PropertyAttribute
    {
        protected ClassTypeConstraintAttribute(Type assemblyType)
        {
            AssemblyType = assemblyType;
        }

        /// <summary>
        /// Get all proper types from executing assembly.
        /// </summary>
        /// <returns></returns>
        public List<Type> GetFilteredTypes()
        {
            var types = new List<Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                types.AddRange(GetFilteredAssemblyTypes(assembly));
            }

            types.Sort((a, b) => a.FullName.CompareTo(b.FullName));

            return types;
        }

        /// <summary>
        /// Get all filtered type from provided assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public List<Type> GetFilteredAssemblyTypes(Assembly assembly)
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
            return AllowAbstract || !type.IsAbstract;
        }


        /// <summary>
        /// Gets or sets whether abstract classes can be selected from drop-down.
        /// Defaults to a value of <c>false</c> unless explicitly specified.
        /// </summary>
        public bool AllowAbstract { get; private set; }

        /// <summary>
        /// Gets or sets grouping of selectable classes. Defaults to <see cref="ClassGrouping.None"/>
        /// unless explicitly specified.
        /// </summary>
        public ClassGrouping Grouping { get; private set; } = ClassGrouping.None;

        /// <summary>
        /// Associated type which will define what type we are looking for.
        /// </summary>
        public Type AssemblyType { get; private set; }
    }

    /// <summary>
    /// Constraint that allows selection of classes that extend a specific class when
    /// selecting a <see cref="SerializedType"/> with the Unity inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ClassExtendsAttribute : ClassTypeConstraintAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExtendsAttribute"/> class.
        /// </summary>
        public ClassExtendsAttribute() : base(null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExtendsAttribute"/> class.
        /// </summary>
        /// <param name="baseType">Type of class that selectable classes must derive from.</param>
        public ClassExtendsAttribute(Type baseType) : base(baseType)
        { }


        /// <inheritdoc/>
        public override bool IsConstraintSatisfied(Type type)
        {
            if (type == AssemblyType || !base.IsConstraintSatisfied(type))
            {
                return false;
            }

            return AssemblyType.IsGenericType
                ? AssemblyType.IsAssignableFromRawGeneric(type)
                : AssemblyType.IsAssignableFrom(type);
        }
    }

    /// <summary>
    /// Constraint that allows selection of classes that implement a specific interface
    /// when selecting a <see cref="SerializedType"/> with the Unity inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ClassImplementsAttribute : ClassTypeConstraintAttribute
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


        /// <inheritdoc/>
        public override bool IsConstraintSatisfied(Type type)
        {
            if (base.IsConstraintSatisfied(type))
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (interfaceType == AssemblyType) return true;
                }
            }

            return false;
        }
    }
}