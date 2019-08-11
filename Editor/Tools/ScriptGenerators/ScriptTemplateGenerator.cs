// Copyright (c) Rotorz Limited. All rights reserved.
// https://bitbucket.org/rotorz/script-templates-for-unity/src/master/

using UnityEditor;

namespace Toolbox.Editor.Tools
{
    public enum VisibilityType
    {
        Public,
        Private,
        Internal,
    }

    /// <summary>
    /// Base class for a script template generator.
    /// </summary>
    /// <remarks>
    /// <para>Custom template generators must be marked with the <see cref="ScriptTemplateAttribute"/>
    /// so that they can be used within the "Create Script" window.</para>
    /// </remarks>
    [InitializeOnLoad]
    public abstract class ScriptTemplateGenerator
    {
        static ScriptTemplateGenerator()
        {
            LoadSharedPreferences();
        }

        #region Shared Preferences

        private static bool bracesOnNewLine;

        private static bool editorScript;
        private static bool initializeOnLoad;
        private static bool staticConstructor;

        private static bool staticClass;
        private static bool partialClass;

        private static VisibilityType visibilityType;

        /// <summary>
        /// Loads all shared preferences from <see cref="EditorPrefs"/> file.
        /// </summary>
        private static void LoadSharedPreferences()
        {
            bracesOnNewLine = EditorPrefs.GetBool("ScriptGenerators.Shared.BracesOnNewLine", false);

            editorScript = EditorPrefs.GetBool("ScriptGenerators.Shared.EditorScript", false);

            initializeOnLoad = EditorPrefs.GetBool("ScriptGenerators.Shared.InitializeOnLoad", false);
            staticConstructor = EditorPrefs.GetBool("ScriptGenerators.Shared.StaticConstructor", false);

            staticClass = EditorPrefs.GetBool("ScriptGenerators.Shared.StaticClass", false);
            partialClass = EditorPrefs.GetBool("ScriptGenerators.Shared.PartialClass", false);

            visibilityType = (VisibilityType)EditorPrefs.GetInt("ScriptGenerators.Shared.TypeVisibility", (int)VisibilityType.Public);
        }

        /// <summary>
        /// Gets or sets whether opening brace character '{' should begin on a new line.
        /// </summary>
        protected static bool BracesOnNewLine
        {
            get { return bracesOnNewLine; }
            set
            {
                if (value == bracesOnNewLine) return;
                EditorPrefs.SetBool("ScriptGenerators.Shared.BracesOnNewLine", bracesOnNewLine = value);
            }
        }

        /// <summary>
        /// When applicable indicates whether script should be for editor usage.
        /// </summary>
        protected static bool IsEditorScript
        {
            get { return editorScript; }
            set
            {
                if (value == editorScript) return;
                EditorPrefs.SetBool("ScriptGenerators.Shared.EditorScript", editorScript = value);
                OutputInitializeOnLoad = value;
            }
        }

        /// <summary>
        /// Indicates that output class should be marked with <c>InitializeOnLoad</c> attribute.
        /// </summary>
        protected static bool OutputInitializeOnLoad
        {
            get { return initializeOnLoad; }
            set
            {
                if (value == initializeOnLoad) return;
                EditorPrefs.SetBool("ScriptGenerators.Shared.InitializeOnLoad", initializeOnLoad = value);
            }
        }

        /// <summary>
        /// Indicates if static constructor should be added to output class.
        /// </summary>
        protected static bool OutputStaticConstructor
        {
            get { return staticConstructor; }
            set
            {
                if (value == staticConstructor) return;
                EditorPrefs.SetBool("ScriptGenerators.Shared.StaticConstructor", staticConstructor = value);
                OutputInitializeOnLoad = value;
            }
        }

        /// <summary>
        /// Indicates whether output class should be marked as static.
        /// </summary>
        protected static bool StaticClass
        {
            get { return staticClass; }
            set
            {
                if (value == staticClass) return;
                EditorPrefs.SetBool("ScriptGenerators.Shared.StaticClass", staticClass = value);
                PartialClass = !value;
            }
        }

        /// <summary>
        /// Indicates whether output class should be marked as partial.
        /// </summary>
        protected static bool PartialClass
        {
            get { return partialClass; }
            set
            {
                if (value == partialClass) return;
                EditorPrefs.SetBool("ScriptGenerators.Shared.PartialClass", partialClass = value);
                StaticClass = !value;
            }
        }

        /// <summary>
        /// Visibility for output type.
        /// </summary>
        protected static VisibilityType VisibilityType
        {
            get { return visibilityType; }
            set
            {
                if (value == visibilityType) return;
                EditorPrefs.SetInt("ScriptGenerators.Shared.TypeVisibility", (int)(visibilityType = value));
            }
        }

        /// <summary>
        /// Gets characters for opening brace insertion.
        /// </summary>
        protected static string OpeningBraceInsertion
        {
            get { return BracesOnNewLine ? "\n{" : " {"; }
        }

        #endregion

        /// <summary>
        /// Draws interface and handles GUI events allowing user to provide additional inputs.
        /// </summary>
        public virtual void OnGUI()
        { }

        /// <summary>
        /// Draws interface and handles GUI events for standard option inputs.
        /// </summary>
        public void OnStandardGUI()
        {
            BracesOnNewLine = EditorGUILayout.ToggleLeft("Places braces on new lines", BracesOnNewLine);
        }

        /// <summary>
        /// Create new <see cref="ScriptBuilder"/> instance to build script.
        /// </summary>
        /// <returns>
        /// The <see cref="ScriptBuilder"/> instance.
        /// </returns>
        public ScriptBuilder CreateScriptBuilder()
        {
            return new ScriptBuilder();
        }

        /// <summary>
        /// Generate C# source code for template.
        /// </summary>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="namespaceName">Namespace for the new script.</param>
        /// <returns>
        /// The generated source code.
        /// </returns>
        /// <seealso cref="CreateScriptBuilder"/>
        public abstract string GenerateScript(string scriptName, string namespaceName);

        /// <summary>
        /// Gets a value indicating whether an editor script will be generated.
        /// </summary>
        public abstract bool WillGenerateEditorScript { get; }
    }
}