using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PresetAttribute : PropertyAttribute
    {
        public PresetAttribute(string presetPropertyName)
        {
            PresetPropertyName = presetPropertyName;
        }

        public string PresetPropertyName { get; private set; }
    }
}