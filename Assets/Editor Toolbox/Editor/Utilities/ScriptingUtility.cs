using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Build;

namespace Toolbox.Editor
{
    public static class ScriptingUtility
    {
        public static List<string> GetDefines()
        {
#if UNITY_2023_1_OR_NEWER
            var target = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var defines = PlayerSettings.GetScriptingDefineSymbols(target);
#else
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif
            return defines.Split(';').ToList();
        }

        public static void SetDefines(List<string> definesList)
        {
            var defines = string.Join(";", definesList.ToArray());
#if UNITY_2023_1_OR_NEWER
            var target = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            PlayerSettings.SetScriptingDefineSymbols(target, defines);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
#endif
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