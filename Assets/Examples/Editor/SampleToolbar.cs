using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

using Toolbox.Editor;

[InitializeOnLoad]
public static class SampleToolbar
{
    private readonly static string mySampleSceneName = "SampleScene";

    private readonly static ToolbarButton myCustomButton;


    static SampleToolbar()
    {
        myCustomButton = new ToolbarButton(() => Debug.Log("Toolbar Test"), new GUIContent("1"));

        EditorSceneManager.sceneOpened -= SceneOpenedCallback;
        EditorSceneManager.sceneOpened += SceneOpenedCallback;

        EditorApplication.update -= ValidateFirstScene;
        EditorApplication.update += ValidateFirstScene;
    }


    private static void ValidateFirstScene()
    {
        if (string.IsNullOrEmpty(SceneManager.GetActiveScene().name))
        {
            return;
        }

        EditorApplication.update -= ValidateFirstScene;

        SceneOpenedCallback(SceneManager.GetActiveScene(), OpenSceneMode.Single);
    }

    private static void SceneOpenedCallback(Scene scene, OpenSceneMode mode)
    {
        if (scene.name == mySampleSceneName)
        {
            ToolboxEditorToolbar.AddToolbarButton(myCustomButton);
        }
        else
        {
            ToolboxEditorToolbar.RemoveToolbarButton(myCustomButton);
        }
    }
}