using System;

using UnityEngine;

[AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
public class EnumFlagAttribute : PropertyAttribute
{ }