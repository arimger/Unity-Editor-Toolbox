using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Hierarchy
{
    public enum HierarchyObjectDataType
    {
        Icon,
        Toggle,
        Tag,
        Layer,
        Script
    }

    /// <summary>
    /// Base class for all custom, Hierarchy-related labels based on targeted <see cref="GameObject"/>.
    /// </summary>
    public abstract class HierarchyPropertyLabel
    {
        protected GameObject target;


        public virtual void Prepare(GameObject target, Rect availableRect)
        {
            this.target = target;
        }

        public virtual float GetWidth()
        {
            return Style.minWidth;
        }

        public abstract void OnGui(Rect rect);


        /// <summary>
        /// Returns built-in label class associated to provided <see cref="HierarchyObjectDataType"/>.
        /// </summary>
        public static HierarchyPropertyLabel GetPropertyLabel(HierarchyObjectDataType dataType)
        {
            switch (dataType)
            {
                case HierarchyObjectDataType.Icon:
                    return new HierarchyIconLabel();
                case HierarchyObjectDataType.Toggle:
                    return new HierarchyToggleLabel();
                case HierarchyObjectDataType.Tag:
                    return new HierarchyTagLabel();
                case HierarchyObjectDataType.Layer:
                    return new HierarchyLayerLabel();
                case HierarchyObjectDataType.Script:
                    return new HierarchyScriptLabel();
            }

            return null;
        }

        #region Classes: Internal

        private class HierarchyIconLabel : HierarchyPropertyLabel
        {
            public override void OnGui(Rect rect)
            {
                var content = EditorGuiUtility.GetObjectContent(target, typeof(GameObject));
                if (content.image)
                {
                    GUI.Label(rect, content.image);
                }
            }
        }

        private class HierarchyToggleLabel : HierarchyPropertyLabel
        {
            public override void OnGui(Rect rect)
            {
                var content = new GUIContent(string.Empty, "Enable/disable GameObject");
                //NOTE: using EditorGUI.Toggle will cause bug and deselect all hierarchy toggles when you will pick a multi-selected property in the Inspector
                var result = GUI.Toggle(new Rect(rect.x + EditorGUIUtility.standardVerticalSpacing,
                        rect.y,
                        rect.width,
                        rect.height),
                    target.activeSelf, content);

                if (rect.Contains(Event.current.mousePosition))
                {
                    if (result != target.activeSelf)
                    {
                        Undo.RecordObject(target, "SetActive");
                        target.SetActive(result);
                    }
                }
            }
        }

        private class HierarchyTagLabel : HierarchyPropertyLabel
        {
            public override float GetWidth()
            {
                return Style.maxWidth;
            }

            public override void OnGui(Rect rect)
            {
                var content = new GUIContent(target.CompareTag("Untagged") ? string.Empty : target.tag, target.tag);
                EditorGUI.LabelField(rect, content, Style.defaultAlignTextStyle);
            }
        }

        private class HierarchyLayerLabel : HierarchyPropertyLabel
        {
            public override void OnGui(Rect rect)
            {
                var layerMask = target.layer;
                var layerName = LayerMask.LayerToName(layerMask);

                string GetContentText()
                {
                    switch (layerMask)
                    {
                        case 00: return string.Empty;
                        case 05: return layerName;
                        default: return layerMask.ToString();
                    }
                }

                var content = new GUIContent(GetContentText(), layerName + " layer");
                EditorGUI.LabelField(rect, content, Style.centreAlignTextStyle);
            }
        }

        private class HierarchyScriptLabel : HierarchyPropertyLabel
        {
            private static Texture componentIcon;
            private static Texture transformIcon;

            private float baseWidth;
            private float summWidth;

            private bool isHighlighted;

            private Component[] components;


            public override void Prepare(GameObject target, Rect availableRect)
            {
                base.Prepare(target, availableRect);

                baseWidth = Style.minWidth;

                var rect = availableRect;
                rect.xMin = rect.xMax - baseWidth;
                if (rect.Contains(Event.current.mousePosition))
                {
                    isHighlighted = true;
                    components = target.GetComponents<Component>();
                    summWidth = components.Length > 1
                             ? (components.Length - 1) * baseWidth
                             : baseWidth;
                }
                else
                {
                    isHighlighted = false;
                    summWidth = baseWidth;
                }

                componentIcon = componentIcon ?? EditorGUIUtility.IconContent("cs Script Icon").image;
                transformIcon = transformIcon ?? EditorGUIUtility.IconContent("Transform Icon").image;
            }

            public override float GetWidth()
            {
                return summWidth;
            }

            public override void OnGui(Rect rect)
            {
                var tooltip = string.Empty;
                var texture = componentIcon;

                rect.xMin = rect.xMax - baseWidth;

                if (isHighlighted)
                {
                    if (components.Length > 1)
                    {
                        texture = null;
                        tooltip = "Components:\n";
                        //set tooltip based on available components
                        for (var i = 1; i < components.Length; i++)
                        {
                            tooltip += "- " + components[i].GetType().Name;
                            tooltip += i == components.Length - 1 ? "" : "\n";
                        }

                        GUI.Label(rect, new GUIContent(string.Empty, tooltip));

                        rect.xMin -= baseWidth * (components.Length - 2);

                        var iconRect = rect;
                        iconRect.xMin = rect.xMin;
                        iconRect.xMax = rect.xMin + baseWidth;

                        //iterate over available components
                        for (var i = 1; i < components.Length; i++)
                        {
                            var component = components[i];
                            var content = EditorGUIUtility.ObjectContent(component, component.GetType());

                            //draw icon for the current component
                            GUI.Label(iconRect, new GUIContent(content.image));
                            //adjust rect for the next script icon
                            iconRect.x += baseWidth;
                        }

                        return;
                    }
                    else
                    {
                        texture = transformIcon;
                        tooltip = "There is no additional component";
                    }
                }

                GUI.Label(rect, new GUIContent(texture, tooltip));
            }
        }

        #endregion

        protected static class Style
        {
            internal static readonly float minWidth = 17.0f;
            internal static readonly float maxWidth = 60.0f;

            internal static readonly GUIStyle defaultAlignTextStyle;
            internal static readonly GUIStyle centreAlignTextStyle;
            internal static readonly GUIStyle rightAlignTextStyle;

            static Style()
            {
                defaultAlignTextStyle = new GUIStyle(EditorStyles.miniLabel)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 9
#else
                    fontSize = 8
#endif
                };
                centreAlignTextStyle = new GUIStyle(EditorStyles.miniLabel)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 9,
#else
                    fontSize = 8,
#endif
#if UNITY_2019_3_OR_NEWER
                    alignment = TextAnchor.MiddleCenter
#else
                    alignment = TextAnchor.UpperCenter
#endif
                };
                rightAlignTextStyle = new GUIStyle(EditorStyles.miniLabel)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 9,
#else
                    fontSize = 8,
#endif
#if UNITY_2019_3_OR_NEWER
                    alignment = TextAnchor.MiddleRight
#else
                    alignment = TextAnchor.UpperRight
#endif
                };
            }
        }
    }
}