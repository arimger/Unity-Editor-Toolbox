using UnityEditor;

namespace Toolbox.Editor.Utilities
{
    public static class PrefabUtility
    {
        [MenuItem("GameObject/Prefab/Revert Prefab Name", true, -100)]
        public static bool ValidateRevertPrefabName()
        {
            var gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; i++)
            {
                var gameObject = gameObjects[i];
                if (UnityEditor.PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
                {
                    return true;
                }
            }

            return false;
        }

        [MenuItem("GameObject/Prefab/Revert Prefab Name", false, -100)]
        public static void RevertPrefabName()
        {
            var gameObjects = Selection.gameObjects;
            Undo.RecordObjects(gameObjects, "Revert Prefab Name");
            for (int i = 0; i < gameObjects.Length; i++)
            {
                var gameObject = gameObjects[i];
                var prefabObject = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                if (prefabObject == null)
                {
                    continue;
                }

                gameObject.name = prefabObject.name;
            }
        }
    }
}