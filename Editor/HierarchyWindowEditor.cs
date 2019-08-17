using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    public class HierarchyWindowEditor
    {
        static HierarchyWindowEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
        }


        private const float iconWidth = 18.0f;
        private const float iconHeight = 18.0f;

        private const string defaultIconName = "GameObject Icon";

        static void HierarchyItemCB(int instanceID, Rect rect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject)
            {
                var content = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject)).image;
                if (content.name == defaultIconName)
                {
                    return;
                }
                GUI.Label(new Rect(rect.xMax, rect.y, iconWidth, iconHeight), content);
            }
        }
    }
}