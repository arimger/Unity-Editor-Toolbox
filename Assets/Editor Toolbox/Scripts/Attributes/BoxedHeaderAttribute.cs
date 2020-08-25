using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class BoxedHeaderAttribute : PropertyAttribute
    {
        public BoxedHeaderAttribute(string header)
        {
            Header = header;
        }

        public string Header { get; private set; }
    }
}