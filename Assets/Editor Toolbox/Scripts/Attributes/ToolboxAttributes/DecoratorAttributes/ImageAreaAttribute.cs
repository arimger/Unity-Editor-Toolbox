using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates and displays texture as decorator, based on URL/URI.
    /// Target texture is downloaded using a standard HTTP web request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class ImageAreaAttribute : ToolboxDecoratorAttribute
    {
        public ImageAreaAttribute(string url) : this(url, 100.0f)
        { }

        public ImageAreaAttribute(string url, float height)
        {
            Url = url;
            Height = height;
        }

        public string Url { get; private set; }

        public float Height { get; private set; }
    }
}