using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    [CustomEditor(typeof(DefaultAsset), true, isFallback = false)]
    public class FolderEditor : UnityEditor.Editor
    {
        protected virtual void OnEnable()
        { 

        }

        protected virtual void OnDisable()
        {

        }
    }
}