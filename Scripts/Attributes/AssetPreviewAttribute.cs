using System;

using UnityEngine;

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