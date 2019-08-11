using System;

using UnityEngine;

[AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class TagSelectorAttribute : PropertyAttribute
{
    public bool UseDefaultTagFieldDrawer = false;
}