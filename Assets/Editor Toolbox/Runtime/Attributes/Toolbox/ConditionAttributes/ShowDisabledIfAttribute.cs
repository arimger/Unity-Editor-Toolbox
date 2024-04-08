﻿using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Shows and disables serialized field if the provided condition is met, otherwise the field is hidden.
    /// 
    /// <para>Supported sources: fields, properties, and methods.</para>
    /// <para>Supported condition types: <see cref="bool"/>, <see cref="int"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/>, any <see cref="Enum"/>, and <see cref="Object"/> (but has to be compared to a <see cref="bool"/> value).</para>
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class ShowDisabledIfAttribute : ComparisonAttribute
    {
        public ShowDisabledIfAttribute(string sourceHandle, object valueToMatch) : base(sourceHandle, valueToMatch)
        { }
                
        public ShowDisabledIfAttribute(string[] sourceHandles, object[] valueToMatches, bool logicAnd = true) 
            : base(sourceHandles, valueToMatches, logicAnd)
        { }
    }
}