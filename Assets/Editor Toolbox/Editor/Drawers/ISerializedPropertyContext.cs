using System;
using System.Reflection;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public interface ISerializedPropertyContext
    {
        SerializedProperty Property { get; }
        FieldInfo FieldInfo { get; }
        Type Type { get; }
    }
}