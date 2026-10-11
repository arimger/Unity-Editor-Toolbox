using System;
using System.Diagnostics;

namespace UnityEngine
{

    /// <summary>
    /// Enables custom sorting of child properties based on their <see cref="OrderAttribute"/> values.
    /// This attribute acts as a gatekeeper: it must be applied to the parent field or class to activate the sorting mechanism.
    /// 
    /// <para>Supported types: any <see cref="class"/>, <see cref="struct"/>, or <see cref="SerializeReference"/> field.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class SortableAttribute : Attribute
    { }

}