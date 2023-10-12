using Toolbox.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SampleToolbar
{
    /// <summary>
    /// This field will be used to exclude toolbar buttons for all scenes except this one.
    /// </summary>
    private readonly static string mySampleSceneName = "SampleScene";

    static SampleToolbar()
    {
        EditorSceneManager.sceneOpened -= SceneOpenedCallback;
        EditorSceneManager.sceneOpened += SceneOpenedCallback;

        EditorApplication.update -= ValidateFirstScene;
        EditorApplication.update += ValidateFirstScene;
    }

    /// <summary>
    /// This method is used to validate first scene after Editor launch.
    /// </summary>
    private static void ValidateFirstScene()
    {
        if (string.IsNullOrEmpty(SceneManager.GetActiveScene().name))
        {
            return;
        }

        EditorApplication.update -= ValidateFirstScene;
        var activeScene = SceneManager.GetActiveScene();
        SceneOpenedCallback(activeScene, OpenSceneMode.Single);
    }

    /// <summary>
    /// Handle <see cref="ToolbarButton"/>s addition for provided scene.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private static void SceneOpenedCallback(Scene scene, OpenSceneMode mode)
    {
        ToolboxEditorToolbar.OnToolbarGui -= OnToolbarGui;
        if (scene.name != mySampleSceneName)
        {
            return;
        }

        ToolboxEditorToolbar.OnToolbarGui += OnToolbarGui;
    }

    /// <summary>
    /// Layout-based GUI call.
    /// </summary>
    private static void OnToolbarGui()
    {
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("1", Style.commandLeftStyle))
        {
            Debug.Log("1");
        }

        if (GUILayout.Button(Style.sampleContent1, Style.commandMidStyle))
        {
            Debug.Log("2");
        }

        if (GUILayout.Button(Style.sampleContent2, Style.commandMidStyle))
        {
            Debug.Log("3");
        }

        if (GUILayout.Button("4", Style.commandRightStyle))
        {
            Debug.Log("4");
        }
    }

    private static class Style
    {
        internal static readonly GUIStyle commandMidStyle = new GUIStyle("CommandMid")
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove
        };
        internal static readonly GUIStyle commandLeftStyle = new GUIStyle("CommandLeft")
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove
        };
        internal static readonly GUIStyle commandRightStyle = new GUIStyle("CommandRight")
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove
        };

        internal static readonly GUIContent sampleContent1;
        internal static readonly GUIContent sampleContent2;

        static Style()
        {
            sampleContent1 = EditorGUIUtility.IconContent("_Help");
            sampleContent2 = EditorGUIUtility.IconContent("AssemblyLock");
        }
    }
}