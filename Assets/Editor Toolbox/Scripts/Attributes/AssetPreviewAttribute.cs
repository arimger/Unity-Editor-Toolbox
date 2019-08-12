using System;

using UnityEngine;

[AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
public class AssetPreviewAttribute : PropertyAttribute
{
    public AssetPreviewAttribute(float width = 64, float height = 64, bool justPreview = false)
    {
        JustPreview = justPreview;

        Height = height;
        Width = width;
    }

    public bool JustPreview { get; private set; }

    public float Height { get; private set; }
    public float Width { get; private set; }
}