using System;

namespace UnityEngine
{
    /// <summary>
    /// Creates additional preview texture for provided object. Can be used only on <see cref="Object"/> proeprties.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
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