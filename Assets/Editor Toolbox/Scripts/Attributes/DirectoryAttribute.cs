using System;

namespace UnityEngine
{
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
    public class DirectoryAttribute : PropertyAttribute
    {
        /// <param name="relativePath">Relative path from ProjectName/Assets directory</param>
        public DirectoryAttribute(string relativePath = null)
        {
            RelativePath = relativePath;
        }

        public string RelativePath { get; private set; }
    }
}