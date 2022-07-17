using Toolbox.Editor.Wizards;

using UnityEditor;
using UnityEngine;

public class SampleWizard : ToolboxWizard
{
    [SerializeField, InLineEditor]
    private GameObject prefab;
    [SerializeField]
    private string targetName;

    private bool hasInvalidName;

    [MenuItem("GameObject/Sample Wizard")]
    public static void CreateWizard()
    {
        DisplayWizard<SampleWizard>("Create GameObject");
    }

    protected override void OnWizardCreate()
    {
        GameObject go;
        if (prefab != null)
        {
            go = Instantiate(prefab);
            go.name = targetName;
        }
        else
        {
            go = new GameObject(targetName);
        }

        Selection.activeObject = go;
    }

    protected override void OnWizardUpdate()
    {
        hasInvalidName = string.IsNullOrEmpty(targetName);
    }

    protected override void OnWizardGui()
    {
        base.OnWizardGui();
        if (hasInvalidName)
        {
            EditorGUILayout.HelpBox("Name is invalid", MessageType.Error, true);
        }
    }

    protected override void HandleOtherButtons()
    {
        //you can draw more buttons here
    }
}