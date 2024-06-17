using System;
using System.Diagnostics;

namespace Toolbox.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class CreateInWizardAttribute : Attribute
    { }
}