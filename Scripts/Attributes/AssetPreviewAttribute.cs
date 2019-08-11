using System;

using UnityEngine;

[AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
public class AssetPreviewAttribute : PropertyAttribute
{
    public AssetPreviewAttribute(float width = 64, float height = 64, bool justPreview = false)
    {
        Width = width;
        Height = height;
     
        JustPreview = justPreview;
    }

    public bool JustPreview { get; private set; }

    public float Height { get; private set; }
    public float Width { get; private set; }
}