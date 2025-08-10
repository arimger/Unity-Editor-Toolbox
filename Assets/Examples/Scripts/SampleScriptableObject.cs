using System;
using Toolbox.Attributes;
using UnityEngine;

[CreateInWizard]
public class SampleScriptableObject : ScriptableObject
{
    [Serializable]
    public class NestedClass
    {
        public bool var1;
        [ReorderableList]
        public int[] vars;
        public bool var2;
    }

    public bool var1;
    [EnableIf(nameof(var1), true)]
    public int var2;
    public string[] vars1;
    [ReorderableList]
    public int[] vars2;
    public NestedClass var3;
}