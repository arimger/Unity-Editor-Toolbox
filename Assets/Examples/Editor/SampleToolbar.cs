using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

using Toolbox.Editor;

[InitializeOnLoad]
public static class SampleToolbar
{
    /// <summary>
    /// This field will be used to exclude toolbar buttons for all scenes except this one.
    /// </summary>
    private readonly static string mySampleSceneName = "SampleScene";

    /// <summary>
    /// Collection of all <see cref="ToolbarButton"/>s needed in custom scene.
    /// </summary>
    private readonly static ToolbarButton[] myCustomButtons = new ToolbarButton[4];


    static SampleToolbar()
    {
        myCustomButtons[0] = new ToolbarButton(() => Debug.Log("Toolbar Test 1"), new GUIContent("1"));
        myCustomButtons[1] = new ToolbarButton(() => Debug.Log("Toolbar Test 2"), new GUIContent("2"));
        myCustomButtons[2] = new ToolbarButton(() => Debug.Log("Toolbar Test 3"), new GUIContent("3"));
        myCustomButtons[3] = new ToolbarButton(() => Debug.Log("Toolbar Test 4"), new GUIContent("4"));

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

        SceneOpenedCallback(SceneManager.GetActiveScene(), OpenSceneMode.Single);
    }

    /// <summary>
    /// Handle <see cref="ToolbarButton"/>s addition for provided scene.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private static void SceneOpenedCallback(Scene scene, OpenSceneMode mode)
    {
        ToolboxEditorToolbar.RemoveToolbarButtons();

        if (scene.name == mySampleSceneName)
        {
            foreach (var button in myCustomButtons)
            {
                ToolboxEditorToolbar.AddToolbarButton(button);
            }
        }
    }
}