using System.Collections.Generic;

using UnityEditor;

namespace Toolbox.Editor.ContextMenu.Management
{
    using Toolbox.Editor.ContextMenu.Operations;

    [InitializeOnLoad]
    internal static class ToolboxContextMenuManager
    {
        private static readonly List<IContextMenuOperation> registeredOperations;

        static ToolboxContextMenuManager()
        {
            registeredOperations = new List<IContextMenuOperation>()
            {
#if UNITY_2021_3_OR_NEWER
                new CopySerializeReferenceOperation(),
                new PasteSerializeReferenceOperation(),
                new DuplicateSerializeReferenceArrayElementOperation()
#endif
            };

            EditorApplication.contextualPropertyMenu -= OnContextMenuOpening;
            EditorApplication.contextualPropertyMenu += OnContextMenuOpening;
        }

        public static void AppendOpertation(IContextMenuOperation operation)
        {
            registeredOperations.Add(operation);
        }

        public static bool RemoveOperation(IContextMenuOperation operation)
        {
            return registeredOperations.Remove(operation);
        }

        private static void OnContextMenuOpening(GenericMenu menu, SerializedProperty property)
        {
            foreach (var operation in registeredOperations)
            {
                if (!operation.IsVisible(property))
                {
                    continue;
                }

                var label = operation.Label;
                if (!operation.IsEnabled(property))
                {
                    menu.AddDisabledItem(label);
                    continue;
                }

                menu.AddItem(label, false, () =>
                {
                    operation.Perform(property);
                });
            }
        }
    }
}