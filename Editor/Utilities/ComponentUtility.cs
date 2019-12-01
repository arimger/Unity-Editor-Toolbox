using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    public static class ComponentUtility
    {
        private static List<Component> copiedComponents = new List<Component>();


        [MenuItem("CONTEXT/Component/Copy Components", false, priority = 200)]
        private static void Copy()
        {
            copiedComponents.Clear();
            var selectedGameObjects = Selection.gameObjects;

            for (var i = 0; i < selectedGameObjects.Length; i++)
            {
                var componentsOfSelectedTransforms = selectedGameObjects[i].GetComponents<Component>();

                for (var j = 0; j < componentsOfSelectedTransforms.Length; j++)
                {       
                    if (componentsOfSelectedTransforms[j] is Transform)
                        continue;

                    if (componentsOfSelectedTransforms[j] == null || copiedComponents.Contains(componentsOfSelectedTransforms[j]))
                        continue;

                    copiedComponents.Add(componentsOfSelectedTransforms[j]);
                }
            }
        }

        [MenuItem("CONTEXT/Component/Copy Components", true)]
        private static bool ValidateCopy()
        {
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem("CONTEXT/Component/Paste Components", false, priority = 201)]
        private static void Paste()
        {
            var selectedGameobjects = Selection.gameObjects;

            for (int i = 0; i < selectedGameobjects.Length; i++)
            {
                for (int j = 0; j < copiedComponents.Count; j++)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponents[j]);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(selectedGameobjects[i].gameObject);
                }
            }

            copiedComponents.Clear();
        }

        [MenuItem("CONTEXT/Component/Paste Components", true)]
        private static bool ValidatePaste()
        {
            return Selection.gameObjects.Length > 0 && copiedComponents.Count > 0;
        }


        public static void SimulateOnValidate(UnityEngine.Object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var methodInfo = target.GetType().GetMethod("OnValidate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo != null)
            {
                methodInfo.Invoke(target, null);
            }
        }
    }
}