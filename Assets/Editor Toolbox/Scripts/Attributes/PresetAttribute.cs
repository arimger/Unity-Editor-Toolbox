using System;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick values based on the given preset (array/list) collection.
    /// Remark: has to be implemented within classes, structs are not supported.
    /// Supported types: all.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PresetAttribute : PropertyAttribute
    {
        public PresetAttribute(string presetFieldName)
        {
            PresetFieldName = presetFieldName;
        }

        public string PresetFieldName { get; private set; }
    }
}