#if UNITY_2019_3_OR_NEWER
using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates dedicated drawer for fields marked with the <see cref="SerializeReference"/>.
    /// 
    /// <para>Supported types: any serializable type and field with the <see cref="SerializeReference"/> attribute.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class ReferencePickerAttribute : ToolboxSelfPropertyAttribute
    { }
}
#endif