using System;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick project-related directory using built-in tool.
    /// 
    /// <para>Supported types: <see cref="string"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
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