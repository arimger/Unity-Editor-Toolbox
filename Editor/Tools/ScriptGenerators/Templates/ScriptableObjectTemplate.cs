using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tools
{
    /// <summary>
    /// Template generator for editor window script.
    /// </summary>
    [ScriptTemplate("Scriptable Object", 300)]
    public sealed class ScriptableObjectTemplate : ScriptTemplateGenerator
    {
        private bool outputOnEnableMethod;
        private bool outputOnDisableMethod;
        private bool outputOnDestroyMethod;

        /// <summary>
        /// Initialize new <see cref="ScriptableObjectTemplate"/> instance.
        /// </summary>
        public ScriptableObjectTemplate()
        {
            outputOnEnableMethod = EditorPrefs.GetBool("ScriptGenerators.Message.OnEnable", false);
            outputOnDisableMethod = EditorPrefs.GetBool("ScriptGenerators.Message.OnDisable", false);
            outputOnDestroyMethod = EditorPrefs.GetBool("ScriptGenerators.Message.OnDestroy", false);
        }

        private void UpdateEditorPrefs()
        {
            EditorPrefs.SetBool("ScriptGenerators.Message.OnEnable", outputOnEnableMethod);
            EditorPrefs.SetBool("ScriptGenerators.Message.OnDisable", outputOnDisableMethod);
            EditorPrefs.SetBool("ScriptGenerators.Message.OnDestroy", outputOnDestroyMethod);
        }

        /// <inheritdoc/>
        public override void OnGUI()
        {
            GUILayout.Label("Output Options", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            IsEditorScript = EditorGUILayout.ToggleLeft("Editor Script", IsEditorScript);

            EditorGUI.BeginDisabledGroup(!IsEditorScript);
            OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
            EditorGUI.EndDisabledGroup();

            OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor",
                OutputStaticConstructor || OutputInitializeOnLoad);

            EditorGUILayout.Space();

            outputOnEnableMethod = EditorGUILayout.ToggleLeft("OnEnable Method", outputOnEnableMethod);
            outputOnDisableMethod = EditorGUILayout.ToggleLeft("OnDisable Method", outputOnDisableMethod);
            outputOnDestroyMethod = EditorGUILayout.ToggleLeft("OnDestroy Method", outputOnDestroyMethod);
            if (EditorGUI.EndChangeCheck()) UpdateEditorPrefs();
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
            if (IsEditorScript && OutputInitializeOnLoad)
                sb.AppendLine("[InitializeOnLoad]");

            sb.BeginNamespace("public class " + scriptName + " : ScriptableObject" + OpeningBraceInsertion);

            //automatically initialize on load?
            if (IsEditorScript && OutputInitializeOnLoad || OutputStaticConstructor)
                sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

            if (outputOnEnableMethod)
                sb.AppendLine("private void OnEnable()" + OpeningBraceInsertion + "\n}\n");
            if (outputOnDisableMethod)
                sb.AppendLine("private void OnDisable()" + OpeningBraceInsertion + "\n}\n");
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
            get { return IsEditorScript; }
        }
    }
}