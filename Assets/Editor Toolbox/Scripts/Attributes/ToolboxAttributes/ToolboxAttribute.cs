using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Base class for all attributes used within Component Editors.
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    public abstract class ToolboxAttribute : Attribute
    { }
}