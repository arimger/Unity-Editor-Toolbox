using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tests
{
    internal static class TestsUtility
    {
        internal static T LoadAsset<T>(string guid) where T : Object
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}