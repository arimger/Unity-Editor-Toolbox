using System;

using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class BoxedHeaderAttribute : PropertyAttribute
{
    public BoxedHeaderAttribute(string header)
    {
        Header = header;
    }

    public string Header { get; private set; }
}