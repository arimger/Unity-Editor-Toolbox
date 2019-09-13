using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tools
{
    /// <summary>
    /// Template generator for editor menu script.
    /// </summary>
    [ScriptTemplate("Editor Menu", 200)]
    public sealed class EditorMenuTemplate : ScriptTemplateGenerator
    {
        private string menuItem;

        /// <summary>
        /// Initialize new <see cref="EditorMenuTemplate"/> instance.
        /// </summary>
        public EditorMenuTemplate()
        {
            menuItem = EditorPrefs.GetString("ScriptGenerators.Shared.MenuItem", "");
        }

        private void UpdateEditorPrefs()
        {
            EditorPrefs.SetString("ScriptGenerators.Shared.MenuItem", menuItem);
        }

        /// <inheritdoc/>
        public override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            GUILayout.Label("Output Options", EditorStyles.boldLabel);

            EditorGUILayout.PrefixLabel("Menu Item (optional)");
            menuItem = EditorGUILayout.TextField(menuItem);

            EditorGUILayout.Space();

            OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
            OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor || OutputInitializeOnLoad);

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

            sb.BeginNamespace("static class " + scriptName + OpeningBraceInsertion);

            //automatically initialize on load?
            if (OutputInitializeOnLoad || OutputStaticConstructor)
                sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

            if (!string.IsNullOrEmpty(menuItem))
            {
                var menuName = menuItem;
                if (!menuName.Contains("/")) menuName = "Window/" + menuName;

                sb.AppendLine("[MenuItem(\"" + menuName + "\")]");
                sb.AppendLine("private static void " + menuName.Replace("/", "_").Replace(" ", "_") + "()" + OpeningBraceInsertion);
                sb.AppendLine("}\n");
            }

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