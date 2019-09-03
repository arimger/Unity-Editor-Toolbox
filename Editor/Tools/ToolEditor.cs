using UnityEngine;
using UnityEditor;
using UnityTools = UnityEditor.Tools;

//TODO:

namespace Toolbox.Editor.Tools
{
    public abstract class ToolEditor : ComponentEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            UnityTools.hidden = true;
            Target.transform.hideFlags = HideFlags.HideInInspector;
            Target.transform.position = Vector3.zero;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            UnityTools.hidden = false;
        }


        public override void OnInspectorGUI()
        {
            //if (serializedObject.isEditingMultipleObjects)
            //{
            //    EditorGUILayout.LabelField("Tool can not be multi-object edited.", EditorStyles.helpBox);
            //    return;
            //}

            EditorGUILayout.LabelField("Tool Editor", EditorStyles.centeredGreyMiniLabel);
        }


        protected Component Target => target as Component;
    }
}