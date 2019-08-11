using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tools
{
    /// <summary>
    /// Template generator for editor window script. 
    /// </summary>
    [ScriptTemplate("Editor Window", 100)]
    public sealed class EditorWindowTemplate : ScriptTemplateGenerator
    {
        private bool outputOnGUIMethod;
        private bool outputUpdateMethod;
        private bool outputOnEnableMethod;
        private bool outputOnDisableMethod;
        private bool outputOnDestroyMethod;

        private bool utility;

        private string menuItem;

        /// <summary>
        /// Initialize new <see cref="EditorWindowTemplate"/> instance.
        /// </summary>
        public EditorWindowTemplate()
        {
            outputOnGUIMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnGUI", false);
            outputUpdateMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Update", false);
            outputOnEnableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnEnable", false);
            outputOnDisableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDisable", false);
            outputOnDestroyMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDestroy", false);

            utility = EditorPrefs.GetBool("ScriptTemplates.EditorWindow.Utility", false);

            menuItem = EditorPrefs.GetString("ScriptTemplates.Shared.MenuItem", "");
        }

        private void UpdateEditorPrefs()
        {
            EditorPrefs.SetBool("ScriptTemplates.Message.OnGUI", outputOnGUIMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.Update", outputUpdateMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnEnable", outputOnEnableMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnDisable", outputOnDisableMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnDestroy", outputOnDestroyMethod);

            EditorPrefs.SetString("ScriptTemplates.Shared.MenuItem", menuItem);

        }

        /// <inheritdoc/>
        public override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            GUILayout.Label("Output Options:", EditorStyles.boldLabel);

            OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
            OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor || OutputInitializeOnLoad);

            EditorGUILayout.Space();

            outputOnGUIMethod = EditorGUILayout.ToggleLeft("OnGUI Method", outputOnGUIMethod);
            outputUpdateMethod = EditorGUILayout.ToggleLeft("Update Method", outputUpdateMethod);
            outputOnEnableMethod = EditorGUILayout.ToggleLeft("OnEnable Method", outputOnEnableMethod);
            outputOnDisableMethod = EditorGUILayout.ToggleLeft("OnDisable Method", outputOnDisableMethod);
            outputOnDestroyMethod = EditorGUILayout.ToggleLeft("OnDestroy Method", outputOnDestroyMethod);

            EditorGUILayout.Space();

            EditorGUILayout.PrefixLabel("Menu Item (optional)");
            menuItem = EditorGUILayout.TextField(menuItem);

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(menuItem));
            utility = EditorGUILayout.ToggleLeft("Utility Window", utility);
            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {
                menuItem = menuItem.Trim();
                UpdateEditorPrefs();
            }
        }

        /// <inheritdoc/>
        public override string GenerateScript(string scriptName, string namespaceName)
        {
            var sb = CreateScriptBuilder();

            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEditor;");
            sb.AppendLine();
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(namespaceName))
                sb.BeginNamespace("namespace " + namespaceName + OpeningBraceInsertion);

            //automatically initialize on load?
            if (OutputInitializeOnLoad)
                sb.AppendLine("[InitializeOnLoad]");

            sb.BeginNamespace("public class " + scriptName + " : EditorWindow" + OpeningBraceInsertion);

            //automatically initialize on load?
            if (OutputInitializeOnLoad || OutputStaticConstructor)
                sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

            if (!string.IsNullOrEmpty(menuItem))
            {
                string menuName = menuItem;
                if (!menuName.Contains("/"))
                    menuName = "Window/" + menuName;

                string utilityArg = utility ? "true, " : "";

                sb.AppendLine("[MenuItem(\"" + menuName + "\")]");
                sb.AppendLine("private static void ShowWindow()" + OpeningBraceInsertion);
                sb.AppendLine("\tGetWindow<" + scriptName + ">(" + utilityArg + "\"" + menuItem + "\");");
                sb.AppendLine("}\n");
            }

            if (outputOnEnableMethod)
                sb.AppendLine("private void OnEnable()" + OpeningBraceInsertion + "\n}\n");
            if (outputOnDisableMethod)
                sb.AppendLine("private void OnDisable()" + OpeningBraceInsertion + "\n}\n");
            if (outputOnGUIMethod)
                sb.AppendLine("private void OnGUI()" + OpeningBraceInsertion + "\n}\n");
            if (outputUpdateMethod)
                sb.AppendLine("private void Update()" + OpeningBraceInsertion + "\n}\n");
            if (outputOnDestroyMethod)
                sb.AppendLine("private void OnDestroy()" + OpeningBraceInsertion + "\n}\n");

            sb.EndNamespace("}");

            if (!string.IsNullOrEmpty(namespaceName))
                sb.EndNamespace("}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override bool WillGenerateEditorScript
        {
            get { return true; }
        }
    }
}