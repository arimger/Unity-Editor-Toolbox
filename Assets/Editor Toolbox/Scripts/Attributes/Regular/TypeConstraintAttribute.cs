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
}