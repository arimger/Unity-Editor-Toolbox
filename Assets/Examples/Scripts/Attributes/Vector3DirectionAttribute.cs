using System;
using UnityEngine;

/// <summary>
/// Allows to pick valid 3D direction value.
/// <para>Supported types: <see cref="Vector3"/>.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class Vector3DirectionAttribute : PropertyAttribute
{ }