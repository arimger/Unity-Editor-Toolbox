using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    /// <summary>
    /// Class responsible for folder custom icons drawing.
    /// </summary>
    [CustomEditor(typeof(DefaultAsset), true, isFallback = false)]
    public class ProjectWindowEditor : UnityEditor.Editor
    {
        protected virtual void OnEnable()
        { 

        }

        protected virtual void OnDisable()
        {

        }
    }
}