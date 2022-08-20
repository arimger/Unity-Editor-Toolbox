using System.Collections.Generic;
using System.Linq;

using UnityEditor;

namespace Toolbox.Editor
{
    public static class ScriptingUtility
    {
        public static List<string> GetDefines()
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            return defines.Split(';').ToList();
        }

        public static void SetDefines(List<string> definesList)
        {
            var defines = string.Join(";", definesList.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }

        public static void AppendDefine(string define)
        {
            var definesList = GetDefines();
            if (definesList.Contains(define))
            {
                return;
            }

            definesList.Add(define);
            SetDefines(definesList);
        }

        public static void RemoveDefine(string define)
        {
            var definesList = GetDefines();
            if (definesList.RemoveAll(s => s == define) == 0)
            {
                return;
            }

            SetDefines(definesList);
        }
    }
}