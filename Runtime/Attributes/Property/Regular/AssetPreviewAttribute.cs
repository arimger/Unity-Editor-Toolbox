using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Draws additional preview texture for the provided object.
    /// 
    /// <para>Supported types: any <see cref="Object"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class AssetPreviewAttribute : PropertyAttribute
    {
        public AssetPreviewAttribute(float width = 64, float height = 64, bool useLabel = true)
        {
            Width = width;
            Height = height;
            UseLabel = useLabel;
        }

        public float Width { get; private set; }

        public float Height { get; private set; }

        public bool UseLabel { get; private set; }
    }
}