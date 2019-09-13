// Copyright (c) Rotorz Limited. All rights reserved.
// https://bitbucket.org/rotorz/script-templates-for-unity/src/master/

using System.Text;

using UnityEngine;

namespace Toolbox.Editor.Tools
{
    /// <summary>
    /// Helps you to build script files with support for automatically indenting source code.
    /// </summary>
    public sealed class ScriptBuilder
    {
        private StringBuilder stringBuilder = new StringBuilder();

        private int indentLevel = 0;

        private string indent = "";
        private string indentChars = "\t";
        private string indentCharsNewLine = "\n";

        /// <summary>
        /// Clear output and start over.
        /// </summary>
        /// <remarks>
        /// <para>This method also resets indention to 0.</para>
        /// </remarks>
        public void Clear()
        {
            stringBuilder.Length = 0;
            IndentLevel = 0;
        }

        /// <summary>
        /// Append text to script and begin new line.
        /// </summary>
        /// <param name="text">Text.</param>
        public void AppendLine(string text)
        {
            Append(text);
            stringBuilder.Append(indentCharsNewLine);
        }

        /// <summary>
        /// Append blank line to script.
        /// </summary>
        public void AppendLine()
        {
            stringBuilder.Append(indentCharsNewLine);
        }

        /// <summary>
        /// Append text to script.
        /// </summary>
        /// <param name="text">Text.</param>
        public void Append(string text)
        {
            stringBuilder.Append(text.Replace("\n", indentCharsNewLine));
        }

        /// <summary>
        /// Begin namespace scope and automatically indent.
        /// </summary>
        /// <param name="text">Text.</param>
        public void BeginNamespace(string text)
        {
            Append(text);
            stringBuilder.AppendLine();
            IndentLevel++;
            stringBuilder.Append(indentCharsNewLine);
        }

        /// <summary>
        /// End namespace scope and unindent.
        /// </summary>
        /// <param name="text">Text.</param>
        public void EndNamespace(string text)
        {
            IndentLevel--;
            stringBuilder.Append(text.Replace("\n", indentCharsNewLine));
            stringBuilder.AppendLine();
            stringBuilder.Append(indent);
            AppendLine();
        }

        /// <summary>
        /// Gets or sets current indent level within script.
        /// </summary>
        public int IndentLevel
        {
            get { return indentLevel; }
            set
            {
                value = Mathf.Max(0, value);
                if (value != indentLevel)
                {
                    if (value < indentLevel)
                    {
                        stringBuilder.Length -= indentChars.Length;
                    }

                    indentLevel = value;
                    indent = "";
                    for (int i = 0; i < value; ++i)
                    {
                        indent += indentChars;
                    }

                    indentCharsNewLine = "\n" + indent;
                }
            }
        }

        /// <summary>
        /// Gets or sets sequence of characters to use when indenting text.
        /// </summary>
        /// <remarks>
        /// <para>Changing this value will not affect text which has already been appended.</para>
        /// </remarks>
        public string IndentChars
        {
            get { return indentChars; }
            set
            {
                var restoreIndent = indentLevel;
                indentLevel = -1;
                indentChars = value;
                IndentLevel = restoreIndent;
            }
        }

        /// <summary>
        /// Get generated source code as string.
        /// </summary>
        /// <returns>
        /// The string.
        /// </returns>
        public override string ToString()
        {
            return stringBuilder.ToString().Trim() + "\n";
        }
    }
}