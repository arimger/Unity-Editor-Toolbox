using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tools
{
    /// <summary>
    /// Base class for basic C# source files.
    /// </summary>
    public abstract class BasicTemplate : ScriptTemplateGenerator
    {
        private readonly string typeKeyword;

        /// <summary>
        /// Initialize new <see cref="ClassTemplate"/> instance.
        /// </summary>
        /// <param name="typeKeyword">Keyword of type; for instance, 'class'.</param>
        public BasicTemplate(string typeKeyword)
        {
            this.typeKeyword = typeKeyword;
        }

        /// <inheritdoc/>
        public override void OnGUI()
        {
            GUILayout.Label("Output options", EditorStyles.boldLabel);

            IsEditorScript = EditorGUILayout.ToggleLeft("Editor Script", IsEditorScript);
            if (!IsEditorScript) OutputInitializeOnLoad = false;

            if (typeKeyword == "class")
            {
                EditorGUI.BeginDisabledGroup(!IsEditorScript);
                OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
                EditorGUI.EndDisabledGroup();

                OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor || OutputInitializeOnLoad);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PrefixLabel("Visibility");
            VisibilityType = (VisibilityType)EditorGUILayout.EnumPopup(VisibilityType);

            if (typeKeyword == "class")
            {
                StaticClass = EditorGUILayout.ToggleLeft("Static", StaticClass);
                PartialClass = EditorGUILayout.ToggleLeft("Partial", PartialClass);
            }
        }

        /// <inheritdoc/>
        public override string GenerateScript(string scriptName, string namespaceName)
        {
            var sb = CreateScriptBuilder();

            sb.AppendLine("using UnityEngine;");
            if (IsEditorScript)
                sb.AppendLine("using UnityEditor;");
            sb.AppendLine();
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(namespaceName))
                sb.BeginNamespace("namespace " + namespaceName + OpeningBraceInsertion);

            //automatically initialize on load?
            if (typeKeyword == "class" && (IsEditorScript && OutputInitializeOnLoad))
                sb.AppendLine("[InitializeOnLoad]");

            //build type declaration string
            var declaration = new List<string>();

            if (VisibilityType == VisibilityType.Public)
                declaration.Add("public");
            else if (VisibilityType == VisibilityType.Internal)
                declaration.Add("internal");

            if (typeKeyword == "class")
            {
                if (StaticClass)
                {
                    declaration.Add("static");
                }
                else if (PartialClass)
                {
                    declaration.Add("partial");
                }
            }

            declaration.Add(typeKeyword);
            declaration.Add(scriptName);

            sb.BeginNamespace(string.Join(" ", declaration.ToArray()) + OpeningBraceInsertion);

            //automatically initialize on load?
            if (typeKeyword == "class" && (IsEditorScript && OutputInitializeOnLoad || OutputStaticConstructor))
                sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

            sb.EndNamespace("}");
            if (!string.IsNullOrEmpty(namespaceName)) sb.EndNamespace("}");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override bool WillGenerateEditorScript
        {
            get { return IsEditorScript; }
        }
    }
}