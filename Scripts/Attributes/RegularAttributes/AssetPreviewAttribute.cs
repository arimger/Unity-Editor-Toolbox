using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws additional preview texture for the provided object.
    /// Supported types: any <see cref="Object"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AssetPreviewAttribute : PropertyAttribute
    {
        public AssetPreviewAttribute(float width = 64, float height = 64, bool useLabel = true)
        {
            UseLabel = useLabel;

            Height = height;
            Width = width;
        }

        public bool UseLabel { get; private set; }

        public float Height { get; private set; }

        public float Width { get; private set; }
    }
}