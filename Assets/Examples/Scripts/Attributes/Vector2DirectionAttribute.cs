using System;
using UnityEngine;

/// <summary>
/// Allows to pick valid 2D direction value.
/// 
/// <para>Supported types: <see cref="Vector2"/>.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class Vector2DirectionAttribute : PropertyAttribute
{ }