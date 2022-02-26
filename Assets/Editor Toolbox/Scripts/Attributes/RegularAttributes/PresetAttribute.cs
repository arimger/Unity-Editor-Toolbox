using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick values based on the given preset (array/list) collection.
    /// Remark: has to be implemented within classes, structs are not supported.
    /// <example>
    /// <code>
    /// [Preset(nameof(presetValues))]
    /// public int presetTarget;
    /// 
    /// private readonly int[] presetValues = new[] { 1, 2, 3, 4, 5 };
    /// </code>
    /// </example>
    /// <para>Supported sources: fields, properties, and methods.</para>
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class PresetAttribute : PropertyAttribute
    {
        public PresetAttribute(string sourceHandle)
        {
            SourceHandle = sourceHandle;
        }

        public string SourceHandle { get; private set; }
    }
}