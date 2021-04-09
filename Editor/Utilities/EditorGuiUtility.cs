using System;
using System.Collections.Generic;

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
        private static readonly Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();


        public static Texture2D CreateColorTexture()
        {
            return CreateColorTexture(Color.clear);
        }

        public static Texture2D CreateColorTexture(Color color)
        {
            var texture = new Texture2D(1, 1);

            texture.SetPixel(0, 0, color);
            texture.hideFlags = HideFlags.HideAndDontSave;
            texture.Apply();

            return texture;
        }

        public static Texture2D GetEditorTexture(string path)
        {
            if (loadedTextures.TryGetValue(path, out var loadedTexture))
            {
                return loadedTexture;
            }
            else
            {
                return loadedTextures[path] = EditorGUIUtility.Load(path) as Texture2D;
            }
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
                content.image = IsDefaultObjectIcon(content.image.name) ||
                                IsDefaultPrefabIcon(content.image.name) ? null : content.image;
            }

            return content;
        }

        public static bool IsDefaultObjectIcon(string name)
        {
            return name == "GameObject Icon" || name == "d_GameObject Icon";
        }

        public static bool IsDefaultObjectIcon(Texture texture)
        {
            return texture != null && IsDefaultObjectIcon(texture.name);
        }

        public static bool IsDefaultPrefabIcon(string name)
        {
            return name == "Prefab Icon" || name == "d_Prefab Icon";
        }

        public static bool IsDefaultPrefabIcon(Texture texture)
        {
            return texture != null && IsDefaultPrefabIcon(texture.name);
        }

        /// <summary>
        /// Returns icon associated to the <see cref="MessageType"/>.
        /// </summary>
        public static Texture GetHelpIcon(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Info:
                    return EditorGUIUtility.IconContent("console.infoicon").image;
                case MessageType.Warning:
                    return EditorGUIUtility.IconContent("console.warnicon").image;
                case MessageType.Error:
                    return EditorGUIUtility.IconContent("console.erroricon").image;
            }

            return null;
        }
    }
}