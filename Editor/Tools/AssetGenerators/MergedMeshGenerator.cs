using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Tools
{
    public class MergedMeshGenerator : ScriptableWizard
    {
        public Mesh mesh;


        [MenuItem("GameObject/Toolbox/Merge Mesh", priority = 0)]
        private static void CreateWizard()
        {
            DisplayWizard("Generate Merged Mesh", typeof(MergedMeshGenerator));
        }


        private void OnWizardCreate()
        {
            if (mesh == null) return;

            mesh.SetTriangles(mesh.triangles, 0);
            mesh.subMeshCount = 1;
        }
    }
}