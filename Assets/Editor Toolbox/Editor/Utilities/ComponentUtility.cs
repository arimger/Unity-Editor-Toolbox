using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEditor.SceneManagement;
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
                var gameObject = selectedGameobjects[i];
                Undo.RecordObjects(selectedGameobjects, "Copy components to selected GameObjects");

                for (int j = 0; j < copiedComponents.Count; j++)
                {
                    var component = copiedComponents[j];
                    UnityEditorInternal.ComponentUtility.CopyComponent(component);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(gameObject);
                }
            }

            copiedComponents.Clear();
        }

        [MenuItem("CONTEXT/Component/Paste Components", true)]
        private static bool ValidatePaste()
        {
            return Selection.gameObjects.Length > 0 && copiedComponents.Count > 0;
        }

        [MenuItem("CONTEXT/Component/Hide Component", false, priority = 300)]
        private static void Hide(MenuCommand menuCommand)
        {
            var component = menuCommand.context as Component;
            Undo.RecordObject(component, "Hide " + component.GetType() + " in " + component.name);
            component.hideFlags |= HideFlags.HideInInspector;
            EditorUtility.SetDirty(component);
        }
        
        [MenuItem("CONTEXT/Component/Hide All Components", false, priority = 301)]
        private static void HideAll(MenuCommand menuCommand)
        {
            var gameObject = (menuCommand.context as Component).gameObject;
            var components = (menuCommand.context as Component).GetComponents<Component>();

            Undo.RecordObject(gameObject, "Hide all components in " + gameObject.name);

            for (var i = 0; i < components.Length; i++)
            {
                components[i].hideFlags |= HideFlags.HideInInspector;
            }

            EditorUtility.SetDirty(gameObject);
        }

        [MenuItem("CONTEXT/Component/Show All Components", false, priority = 302)]
        private static void ShowAll(MenuCommand menuCommand)
        {
            var gameObject = (menuCommand.context as Component).gameObject;
            var components = (menuCommand.context as Component).GetComponents<Component>();

            Undo.RecordObject(gameObject, "Show all components in " + gameObject.name);

            for (var i = 0; i < components.Length; i++)
            {
                components[i].hideFlags &= ~HideFlags.HideInInspector;
            }

            EditorUtility.SetDirty(gameObject);
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