using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tools
{
    /// <summary>
    /// Template generator for MonoBehaviour script.
    /// </summary>
    [ScriptTemplate("Mono Behaviour", 0)]
    public sealed class MonoBehaviourTemplate : ScriptTemplateGenerator
    {
        private bool outputAwakeMethod;
        private bool outputStartMethod;
        private bool outputUpdateMethod;
        private bool outputDestroyMethod;

        /// <summary>
        /// Initialize new <see cref="MonoBehaviourTemplate"/> instance.
        /// </summary>
        public MonoBehaviourTemplate()
        {
            outputAwakeMethod = EditorPrefs.GetBool("ScriptGenerators.Message.Awake", false);
            outputStartMethod = EditorPrefs.GetBool("ScriptGenerators.Message.Start", true);
            outputUpdateMethod = EditorPrefs.GetBool("ScriptGenerators.Message.Update", true);
            outputDestroyMethod = EditorPrefs.GetBool("ScriptGenerators.Message.OnDestroy", false);
        }

        private void UpdateEditorPrefs()
        {
            EditorPrefs.SetBool("ScriptGenerators.Message.Awake", outputAwakeMethod);
            EditorPrefs.SetBool("ScriptGenerators.Message.Start", outputStartMethod);
            EditorPrefs.SetBool("ScriptGenerators.Message.Update", outputUpdateMethod);
            EditorPrefs.SetBool("ScriptGenerators.Message.OnDestroy", outputDestroyMethod);
        }

        /// <inheritdoc/>
        public override void OnGUI()
        {
            GUILayout.Label("Output Options", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor);

            EditorGUILayout.Space();

            outputAwakeMethod = EditorGUILayout.ToggleLeft("Awake Method", outputAwakeMethod);
            outputStartMethod = EditorGUILayout.ToggleLeft("Start Method", outputStartMethod);
            outputUpdateMethod = EditorGUILayout.ToggleLeft("Update Method", outputUpdateMethod);
            outputDestroyMethod = EditorGUILayout.ToggleLeft("OnDestroy Method", outputDestroyMethod);

            if (EditorGUI.EndChangeCheck()) UpdateEditorPrefs();
        }

        /// <inheritdoc/>
        public override string GenerateScript(string scriptName, string namespaceName)
        {
            var sb = CreateScriptBuilder();

            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(namespaceName))
            {
                sb.BeginNamespace("namespace " + namespaceName + OpeningBraceInsertion);
            }

            sb.BeginNamespace("public class " + scriptName + " : MonoBehaviour" + OpeningBraceInsertion);

            //automatically initialize on load?
            if (OutputStaticConstructor)
                sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

            if (outputAwakeMethod)
                sb.AppendLine("private void Awake()" + OpeningBraceInsertion + "\n}\n");
            if (outputStartMethod)
                sb.AppendLine("private void Start()" + OpeningBraceInsertion + "\n}\n");
            if (outputUpdateMethod)
                sb.AppendLine("private void Update()" + OpeningBraceInsertion + "\n}\n");
            if (outputDestroyMethod)
                sb.AppendLine("private void OnDestroy()" + OpeningBraceInsertion + "\n}\n");

            sb.EndNamespace("}");

            if (!string.IsNullOrEmpty(namespaceName)) sb.EndNamespace("}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override bool WillGenerateEditorScript
        {
            get { return false; }
        }
    }
}