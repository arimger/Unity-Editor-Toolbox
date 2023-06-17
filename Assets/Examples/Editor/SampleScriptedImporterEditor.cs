#if UNITY_2020_2_OR_NEWER
using Toolbox.Editor.Editors;
using UnityEditor;

[CustomEditor(typeof(SampleScriptedImporter))]
public class SampleScriptedImporterEditor : ToolboxScriptedImporterEditor
{ }
#endif