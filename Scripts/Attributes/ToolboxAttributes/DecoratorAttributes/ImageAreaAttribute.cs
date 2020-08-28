using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
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
