using System;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    /// <summary>
    /// Internal overlay on the built-in <see cref="EditorGUIUtility"/> class.
    /// </summary>
    internal static class EditorGuiUtility
    {
        public static Texture2D CreatePersistantTexture()
        {
            return CreatePersistantTexture(Color.clear);
        }

        public static Texture2D CreatePersistantTexture(Color color)
        {
            var mode = FilterMode.Point;
            var flag = HideFlags.HideAndDontSave;
            var texture = new Texture2D(1, 1);

            texture.SetPixel(0, 0, color);
            texture.filterMode = mode;
            texture.hideFlags = flag;
            texture.Apply();

            return texture;
        }

        public static GUIContent GetObjectContent(Object target, Type targetType)
        {
            return GetObjectContent(target, targetType, true);
        }

        public static GUIContent GetObjectContent(Object target, Type targetType, bool clearDefaults)
        {
            var content = EditorGUIUtility.ObjectContent(target, targetType);
            if (content.image && clearDefaults)
            {
                content.image = IsDefaultObjectIcon(content.image) ? null : content.image;
                content.image = IsDefaultPrefabIcon(content.image) ? null : content.image;
            }

            return content;
        }

        public static Texture2D GetIconContent(string name)
        {
            return EditorGUIUtility.IconContent(name).image as Texture2D;
        }

        public static bool IsDefaultObjectIcon(string name)
        {
            return name == "GameObject Icon";
        }

        public static bool IsDefaultObjectIcon(Texture texture)
        {
            return texture != null && IsDefaultObjectIcon(texture.name);
        }

        public static bool IsDefaultPrefabIcon(string name)
        {
            return name == "Prefab Icon";
        }

        public static bool IsDefaultPrefabIcon(Texture texture)
        {
            return texture != null && IsDefaultPrefabIcon(texture.name);
        }
    }
}