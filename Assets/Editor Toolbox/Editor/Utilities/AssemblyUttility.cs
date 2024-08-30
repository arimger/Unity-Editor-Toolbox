using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    public static class AssemblyUttility
    {
        private static readonly HashSet<string> projectAssemblyNames = new HashSet<string>();
        private static readonly HashSet<string> internalAssemblyNames = new HashSet<string>()
        {
            "Bee.BeeDriver",
            "ExCSS.Unity",
            "Mono.Security",
            "mscorlib",
            "netstandard",
            "Newtonsoft.Json",
            "nunit.framework",
            "ReportGeneratorMerged",
            "Unrelated",
            "SyntaxTree.VisualStudio.Unity.Bridge",
            "SyntaxTree.VisualStudio.Unity.Messaging",
        };

        private static bool isCached;

        static AssemblyUttility()
        {
            isCached = false;
        }

        private static void CacheProjectAssemblies()
        {
            projectAssemblyNames.Clear();
            var appDomain = AppDomain.CurrentDomain;
            foreach (var assembly in appDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                {
                    continue;
                }

                var assemblyName = assembly.GetName().Name;
                if (assemblyName.StartsWith("System") ||
                   assemblyName.StartsWith("Unity") ||
                   assemblyName.StartsWith("UnityEditor") ||
                   assemblyName.StartsWith("UnityEngine") ||
                   internalAssemblyNames.Contains(assemblyName))
                {
                    continue;
                }

                projectAssemblyNames.Add(assembly.FullName);
            }
        }

        public static bool IsProjectAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                return false;
            }

            return IsProjectAssembly(assembly.FullName);
        }

        public static bool IsProjectAssembly(string assemblyName)
        {
            return ProjectAssemblyNames.Contains(assemblyName);
        }

        public static HashSet<string> ProjectAssemblyNames
        {
            get
            {
                if (!isCached)
                {
                    CacheProjectAssemblies();
                    isCached = true;
                }

                return projectAssemblyNames;
            }
        }
    }
}