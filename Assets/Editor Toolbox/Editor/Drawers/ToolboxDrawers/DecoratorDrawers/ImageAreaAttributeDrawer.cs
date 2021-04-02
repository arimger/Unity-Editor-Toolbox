using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;

namespace Toolbox.Editor.Drawers
{
    public class ImageAreaAttributeDrawer : ToolboxDecoratorDrawer<ImageAreaAttribute>
    {
        private readonly static Dictionary<string, DownloadedTexture> textures = new Dictionary<string, DownloadedTexture>();


        protected override void OnGuiBeginSafe(ImageAreaAttribute attribute)
        {
            var url = attribute.Url;
            if (!textures.TryGetValue(url, out var texture) || texture.IsInvalid)
            {
                textures[url] = texture = new DownloadedTexture(true);
                EditorCoroutineUtility.StartCoroutineOwnerless(SendGetImageRequest(url, (b, t) =>
                {
                    textures[url] = new DownloadedTexture(false, t);
                    if (b)
                    {
                        InspectorUtility.RepaintInspectors();
                    }
                    else
                    {
                        ToolboxEditorLog.AttributeUsageWarning(attribute, "Cannot retrive image from the provided URL - " + url);
                    }
                }));
            }

            if (texture.Texture2D != null)
            {
                EditorGUILayout.LabelField(new GUIContent(texture.Texture2D), Style.imageStyle, GUILayout.Height(attribute.Height));
            }
        }


        private UnityWebRequest GetImageRequest(string url)
        {
            return UnityWebRequestTexture.GetTexture(url, false);
        }

        private IEnumerator SendGetImageRequest(string url, Action<bool, Texture2D> onRequestResult)
        {
            var webRequest = GetImageRequest(url);
            yield return webRequest.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (webRequest.result != UnityWebRequest.Result.Success)
#else
            if (webRequest.isHttpError || webRequest.isNetworkError)
#endif
            {
                onRequestResult?.Invoke(false, null);
            }
            else
            {
                onRequestResult?.Invoke(true, (webRequest.downloadHandler as DownloadHandlerTexture).texture);
            }
        }


        private class DownloadedTexture
        {
            public DownloadedTexture(bool isLoading) : this(isLoading, null)
            { }

            public DownloadedTexture(bool isLoading, Texture2D texture2D)
            {
                IsLoading = isLoading;
                Texture2D = texture2D;
            }

            public bool IsLoaded
            {
                get => !IsLoading && Texture2D;
            }

            public bool IsLoading
            {
                get; set;
            }

            public bool IsInvalid
            {
                get => !IsLoading && !Texture2D;
            }

            public Texture2D Texture2D
            {
                get; set;
            }
        }


        internal static class Style
        {
            internal static readonly GUIStyle imageStyle;

            static Style()
            {
                imageStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter
                };
            }
        }
    }
}