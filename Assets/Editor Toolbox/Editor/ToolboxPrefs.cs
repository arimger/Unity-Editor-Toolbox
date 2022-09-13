using System;
using System.Collections.Generic;

using UnityEditor;

namespace Toolbox.Editor
{
    internal static class ToolboxPrefs
    {
        private const string separator = ".";
        private const string keyBase = "Toolbox";

        private static readonly HashSet<string> usedKeys = new HashSet<string>();


        private static string GetKey(object causer, string propertyName)
        {
            return GetKey(causer.GetType(), propertyName);
        }

        private static string GetKey(Type causer, string propertyName)
        {
            return GetKey(causer.Name, propertyName);
        }

        private static string GetKey(string causer, string propertyName)
        {
            return string.Format("{0}{3}{1}{3}{2}", keyBase, causer, propertyName, separator);
        }


        public static void DeleteAll()
        {
            EditorPrefs.DeleteAll();
        }

        public static void DeleteCached()
        {
            //TODO:
        }

        public static bool GetBool(string causer, string propertyName, bool defaultValue)
        {
            var key = GetKey(causer, propertyName);
            return EditorPrefs.GetBool(key, defaultValue);
        }

        public static T GetValue<T>(string causer, string propertyName, T defaultValue)
        {
            return default;
        }
    }
}