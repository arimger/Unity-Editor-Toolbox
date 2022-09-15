using System;

using UnityEditor;

namespace Toolbox.Editor
{
    /// <summary>
    /// Additional overlay for <see cref="EditorPrefs"/>.
    /// This utility class is Work in Progress.
    /// </summary>
    internal static class ToolboxPrefs
    {
        private static string GetKey(Type causer, string propertyName)
        {
            return GetKey(causer.Name, propertyName);
        }

        private static string GetKey(string causer, string propertyName)
        {
            return string.Format("{0}{3}{1}{3}{2}", "Toolbox", causer, propertyName, ".");
        }


        public static void DeleteAll()
        {
            EditorPrefs.DeleteAll();
        }

        public static bool GetBool(object causer, string propertyName, bool defaultValue = false)
        {
            return GetBool(causer.GetType(), propertyName, defaultValue);
        }

        public static bool GetBool(Type causer, string propertyName, bool defaultValue = false)
        {
            var key = GetKey(causer, propertyName);
            return EditorPrefs.GetBool(key, defaultValue);
        }

        public static void SetBool(object causer, string propertyName, bool value)
        {
            SetBool(causer.GetType(), propertyName, value);
        }

        public static void SetBool(Type causer, string propertyName, bool value)
        {
            var key = GetKey(causer, propertyName);
            EditorPrefs.SetBool(key, value);
        }

        public static int GetInt(object causer, string propertyName, int defaultValue = 0)
        {
            return GetInt(causer.GetType(), propertyName, defaultValue);
        }

        public static int GetInt(Type causer, string propertyName, int defaultValue = 0)
        {
            var key = GetKey(causer, propertyName);
            return EditorPrefs.GetInt(key, defaultValue);
        }

        public static void SetInt(object causer, string propertyName, int value)
        {
            SetInt(causer.GetType(), propertyName, value);
        }

        public static void SetInt(Type causer, string propertyName, int value)
        {
            var key = GetKey(causer, propertyName);
            EditorPrefs.SetInt(key, value);
        }
    }
}