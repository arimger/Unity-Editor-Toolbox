using System;

using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ReadOnlyFieldAttribute : PropertyAttribute
{ }