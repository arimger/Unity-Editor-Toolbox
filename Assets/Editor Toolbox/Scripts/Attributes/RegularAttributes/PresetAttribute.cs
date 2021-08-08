using System;

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
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PresetAttribute : PropertyAttribute
    {
        public PresetAttribute(string sourceName)
        {
            SourceName = sourceName;
        }

        public string SourceName { get; private set; }
    }
}