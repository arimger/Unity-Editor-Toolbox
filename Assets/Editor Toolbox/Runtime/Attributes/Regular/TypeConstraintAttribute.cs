using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick proper type using popup control.
    /// 
    /// <para>Supported types: <see cref="SerializedType"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class TypeConstraintAttribute : PropertyAttribute
    {
        public TypeConstraintAttribute(Type assemblyType)
        {
            AssemblyType = assemblyType;
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
        /// Indicates if types should be sorted alphabetically.
        /// </summary>
        public bool OrderTypes { get; set; } = true;

        /// <summary>
        /// Gets or sets grouping of selectable classes.
        /// Defaults to <see cref="ClassGrouping.None"/> unless explicitly specified.
        /// </summary>
        [Obsolete("Use TypeGrouping instead.")]
        public ClassGrouping Grouping { get; set; } = ClassGrouping.None;

        /// <summary>
        /// Gets or sets grouping of selectable classes.
        /// Defaults to <see cref="TypeGrouping.None"/> unless explicitly specified.
        /// </summary>
        public TypeGrouping TypeGrouping { get; set; } = TypeGrouping.None;
        /// <summary>
        /// Indicates what kind of types are accepted.
        /// </summary>
        public TypeSettings TypeSettings { get; set; } = TypeSettings.Class | TypeSettings.Interface;
    }

    ///<inheritdoc/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class ClassExtendsAttribute : TypeConstraintAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExtendsAttribute"/> class.
        /// </summary>
        [Obsolete]
        public ClassExtendsAttribute() : base(typeof(object))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExtendsAttribute"/> class.
        /// </summary>
        /// <param name="baseType">Type of class that selectable classes must derive from.</param>
        public ClassExtendsAttribute(Type baseType) : base(baseType)
        {
            TypeSettings = TypeSettings.Class;
        }
    }

    ///<inheritdoc/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class ClassImplementsAttribute : TypeConstraintAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassImplementsAttribute"/> class.
        /// </summary>
        [Obsolete]
        public ClassImplementsAttribute() : base(null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassImplementsAttribute"/> class.
        /// </summary>
        /// <param name="interfaceType">Type of interface that selectable classes must implement.</param>
        public ClassImplementsAttribute(Type interfaceType) : base(interfaceType)
        {
            TypeSettings = TypeSettings.Class;
        }
    }

    /// <summary>
    /// Indicates what kind of <see cref="Type"/>s are accepted by the filtering system.
    /// </summary>
    [Flags]
    public enum TypeSettings
    {
        Class = 1,
        Interface = 2
    }

    /// <summary>
    /// Indicates how selectable classes should be collated in drop-down menu.
    /// </summary>
    public enum TypeGrouping
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
        /// <summary>
        /// Only name of the <see cref="Type"/>.
        /// </summary>
        ByFlatName
    }

    /// <summary>
    /// Indicates how selectable classes should be collated in drop-down menu.
    /// </summary>
    [Obsolete("Use TypeGrouping instead.")]
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