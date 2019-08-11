using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tools
{
    /// <summary>
    /// Main tool window. Provides all functionalities and is responsible for validation. 
    /// </summary>
    public class ScriptEditorWindow : ToolEditorWindow
    {
        /// <summary>
        /// Creates <see cref="ScriptEditorWindow"/> from Unity's build-in menu.
        /// </summary>
        [MenuItem("Window/Tools/Script Template Editor")]
        public static void Init()
        {
            GetWindow<ScriptEditorWindow>("Script Creator").Show();
        }

        /// <summary>
        /// Provided script name.
        /// </summary>
        [SerializeField]
        private string scriptName = "";
        /// <summary>
        /// Provided namespace name.
        /// </summary>
        [SerializeField]
        private string namespaceName = "";

        /// <summary>
        /// Information if script should be created in
        /// sub-folder named like provided namespace.
        /// </summary>
        [SerializeField]
        private bool useNamespaceForSubFolders;

        /// <summary>
        /// Known templates.
        /// </summary>
        [NonSerialized]
        private string[] templateNames;

        /// <summary>
        /// Chosen template index.
        /// </summary>
        [SerializeField]
        private int templateIndex;

        [NonSerialized]
        private ScriptTemplateGenerator chosenGenerator;

        private Vector2 scrollPosition;


        protected override void OnEnable()
        {
            base.OnEnable();

            minSize = new Vector2(200, 100);

            useNamespaceForSubFolders = EditorPrefs.GetBool("ScriptGenerators.NamespaceToDirectory", true);

            AutoFixActiveGenerator();
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            EditorGUILayout.Space();
            DrawTemplateSelector();
            AutoFixActiveGenerator();
            EditorGUILayout.Space();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                DrawStandardInputs();
                EditorGUILayout.Space();
                chosenGenerator.OnGUI();
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndScrollView();
            chosenGenerator.OnStandardGUI();
            DrawButtonsStrip();
            EditorGUILayout.Space();
        }


        private void DrawTemplateSelector()
        {
            if (templateNames == null)
            {
                templateNames = ScriptDescriptor.Descriptors
                    .Select(descriptor => descriptor.Attribute.TemplateName)
                    .ToArray();
            }

            EditorGUILayout.LabelField("Template");
            EditorGUI.BeginChangeCheck();
            templateIndex = EditorGUILayout.Popup(templateIndex, templateNames);
            if (EditorGUI.EndChangeCheck()) chosenGenerator = null;
        }

        private void DrawStandardInputs()
        {
            EditorGUILayout.LabelField("Script Name");
            scriptName = EditorGUILayout.TextField(scriptName);

            EditorGUILayout.LabelField("Namespace");
            namespaceName = EditorGUILayout.TextField(namespaceName);

            EditorGUI.BeginChangeCheck();
            useNamespaceForSubFolders = EditorGUILayout.ToggleLeft("Use namespace for sub-folders", useNamespaceForSubFolders);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("ScriptGenerators.NamespaceToDirectory", useNamespaceForSubFolders);
            }
            ToolboxEditorUtility.DrawLayoutLine();
        }

        private void DrawButtonsStrip()
        {
            ToolboxEditorUtility.DrawLayoutLine();

            if (GUILayout.Button("Save at Default Path"))
            {
                DoSaveAtDefaultPath();
            }

            if (GUILayout.Button("Save As"))
            {
                DoSaveAs();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Copy to Clipboard"))
            {
                DoCopyToClipboard();
            }
        }

        private void AutoFixActiveGenerator()
        {
            if (chosenGenerator != null) return;

            templateIndex = Mathf.Clamp(templateIndex, 0, ScriptDescriptor.Descriptors.Count);
            chosenGenerator = ScriptDescriptor.Descriptors[templateIndex].CreateInstance();
        }

        private void ResetInputs()
        {
            AutoFixActiveGenerator();

            scriptName = "";
            namespaceName = "";
        }

        private void DoSaveAtDefaultPath()
        {
            GenerateScriptFromTemplate(Path.Combine(GetDefaultOutputPath(), scriptName + ".cs"));
        }

        private void DoSaveAs()
        {
            var path = EditorUtility.SaveFilePanel("Save New Script", GetDefaultOutputPath(), scriptName + ".cs", ".cs");
            if (!string.IsNullOrEmpty(path))
            {
                GenerateScriptFromTemplate(path);
            }
        }

        private void ValidateScriptInputs()
        {
            //ensure that valid script name was specified
            if (!Regex.IsMatch(scriptName, @"^[A-za-z_][A-za-z_0-9]*$"))
            {
                EditorUtility.DisplayDialog("Invalid Script Name", string.Format("'{0}' is not a valid type name.", scriptName), "OK");
                return;
            }
            //if a namespace was specified, ensure that it is valid
            if (!string.IsNullOrEmpty(namespaceName) &&
                !Regex.IsMatch(namespaceName, @"^[A-za-z_][A-za-z_0-9]*(\.[A-za-z_][A-za-z_0-9]*)*$"))
            {
                EditorUtility.DisplayDialog("Invalid Namespace", string.Format("'{0}' is not a valid namespace.", namespaceName), "OK");
                return;
            }
        }

        private void GenerateScriptFromTemplate(string path)
        {
            EditorGUIUtility.keyboardControl = 0;
            EditorGUIUtility.editingTextField = false;

            if (!path.EndsWith(".cs"))
            {
                EditorUtility.DisplayDialog("Invalid File Extension", "Could not save script because the wrong file extension was specified.\n\nPlease ensure to save as '.cs' file.", "OK");
                return;
            }
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                EditorUtility.DisplayDialog("Invalid Path", "Could not save script because the specified directory does not exist.", "OK");
                return;
            }

            ValidateScriptInputs();

            var fullName = !string.IsNullOrEmpty(namespaceName)
                ? namespaceName + "." + scriptName
                : scriptName;

            if (!IsClassNameUnique(fullName))
            {
                if (!EditorUtility.DisplayDialog("Warning: Type Already Exists!",
                    string.Format(
                        "A type already exists with the name '{0}'.\n\nIf you proceed" +
                        " then you will get compilation errors in the console window.",
                        fullName), "Proceed", "Cancel"))
                {
                    return;
                }

            }

            //generate source code.
            var sourceCode = chosenGenerator.GenerateScript(scriptName, namespaceName);
            //erite to file
            File.WriteAllText(path, sourceCode, Encoding.UTF8);
            AssetDatabase.Refresh();
            Repaint();
        }

        private void DoCopyToClipboard()
        {
            EditorGUIUtility.keyboardControl = 0;
            EditorGUIUtility.editingTextField = false;

            ValidateScriptInputs();

            EditorGUIUtility.systemCopyBuffer = chosenGenerator.GenerateScript(scriptName, namespaceName);
        }

        private string GetDefaultOutputPath()
        {
            var assetFolderPath = Path.Combine("Assets", chosenGenerator.WillGenerateEditorScript ? "Editor/Scripts" : "Scripts");

            if (useNamespaceForSubFolders && !string.IsNullOrEmpty(namespaceName))
            {
                assetFolderPath = Path.Combine(assetFolderPath, namespaceName.Replace("_", "/"));
            }

            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), assetFolderPath);

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            return outputPath;
        }


        private static bool IsClassNameUnique(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                throw new InvalidOperationException("An empty or null string was specified.");
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.FullName == fullName) return false;
                }
            }

            return true;
        }
    }
}