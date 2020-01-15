/*#define CAPTURE*/

using System.IO;
using System.Linq;

using UnityEngine;
using UnityEditor;
#if CAPTURE
using UnityEngine.SceneManagement;
#endif

namespace Toolbox.Editor.Editors
{
    using Editor = UnityEditor.Editor;

#if CAPTURE
    [CustomEditor(typeof(SceneAsset))]
#endif
    [CanEditMultipleObjects]
    public class ScenePreview : Editor
    {
        private const string previewsDirectoryBase = "";
        private const string previewsDirectoryName = "Previews";
        private const string previewsFileExtension = ".png";

        private const float editorMargin = 50.0f;
        private const float previewMargin = 5.0f;

        private static bool shouldRefreshDatabase;


#if CAPTURE
        [RuntimeInitializeOnLoadMethod]
        private static void CaptureScreenshot()
        {
            var previewDirectoryPath = GetPreviewDirectoryPath();
            if (!Directory.Exists(previewDirectoryPath))
            {
                Directory.CreateDirectory(previewDirectoryPath);
            }
            var previewPath = GetScenePreviewFilePath(SceneManager.GetActiveScene().name);

            ScreenCapture.CaptureScreenshot(previewPath);

            shouldRefreshDatabase = true;
        }
#endif
        private static string GetPreviewDirectoryPath()
        {
            return previewsDirectoryBase + "/" + previewsDirectoryName;
        }

        private static string GetScenePreviewFilePath(string sceneName)
        {
            return GetPreviewDirectoryPath() + "/" + sceneName + previewsFileExtension;
        }

        private static string GetScenePreviewFilePathRelative(string sceneName)
        {
            return previewsDirectoryName + "/" + sceneName + previewsFileExtension;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (shouldRefreshDatabase)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                shouldRefreshDatabase = false;
            }

            var sceneNames = targets.Select(t => t.name).OrderBy(n => n).ToArray();

            var previewsCount = sceneNames.Length;
            var previewWidth = Screen.width;
            var previewHeight = (Screen.height - editorMargin * 2 - (previewMargin * previewsCount)) / previewsCount;

            for (int i = 0; i < sceneNames.Length; i++)
            {
                DrawPreview(i, sceneNames[i], previewWidth, previewHeight);
            }
        }


        private void DrawPreview(int index, string sceneName, float width, float height)
        {           
            var preview = AssetDatabase.LoadAssetAtPath<Texture>(GetScenePreviewFilePathRelative(sceneName));
            if (preview == null)
            {
#if CAPTURE
                EditorGUILayout.HelpBox(
                    "There is no image preview for this scene. Please play the scene on editor and image preview will be captured automatically.",
                    MessageType.Info);
#endif
            }
            else
            {
                GUI.DrawTexture(new Rect(index, editorMargin + index * (height + previewMargin), width, height), preview, ScaleMode.ScaleToFit);
            }
        }
    }
}