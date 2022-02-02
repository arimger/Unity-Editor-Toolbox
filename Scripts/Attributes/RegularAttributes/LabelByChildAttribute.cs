using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Changes label by the current value of targeted child.
    /// <example>
    /// <code>
    /// [System.Serializable]
    /// public class CustomType
    /// {
    ///     public KeyCode foo;
    /// }
    /// 
    /// [LabelByChild("foo")]
    /// public CustomType bar;
    /// </code>
    /// In given example label of the 'bar' field in the Inspector Window will be always equal to the value of the 'foo' field.
    /// </example>
    /// <para>Supported types: any type with children.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class LabelByChildAttribute : PropertyAttribute
    {
        public LabelByChildAttribute(string childName)
        {
            ChildName = childName;
        }

        public string ChildName { get; private set; }
    }
}