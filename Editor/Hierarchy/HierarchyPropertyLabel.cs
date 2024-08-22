using System;
using System.Collections.Generic;
using System.Text;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Hierarchy
{
    //TODO: refactor: replace labels with drawers (similar approach to the Inspector), possibility to define drawers and implement them using a dedicated base class & SerializeReference approach

    /// <summary>
    /// Base class for all custom, Hierarchy-related labels based on targeted <see cref="GameObject"/>.
    /// </summary>
    public abstract class HierarchyPropertyLabel
    {
        protected GameObject target;

        public virtual bool Prepare(GameObject target, Rect availableRect)
        {
            return this.target = target;
        }

        public virtual bool Prepare(GameObject target, Rect availableRect, out float neededWidth)
        {
            if (Prepare(target, availableRect))
            {
                neededWidth = GetWidth();
                return true;
            }
            else
            {
                neededWidth = 0.0f;
                return false;
            }
        }

        public virtual float GetWidth()
        {
            return Style.minWidth;
        }

        public abstract void OnGui(Rect rect);

        /// <summary>
        /// Returns built-in label class associated to provided <see cref="HierarchyItemDataType"/>.
        /// </summary>
        public static HierarchyPropertyLabel GetPropertyLabel(HierarchyItemDataType dataType)
        {
            switch (dataType)
            {
                case HierarchyItemDataType.Icon:
                    return new HierarchyIconLabel();
                case HierarchyItemDataType.Toggle:
                    return new HierarchyToggleLabel();
                case HierarchyItemDataType.Tag:
                    return new HierarchyTagLabel();
                case HierarchyItemDataType.Layer:
                    return new HierarchyLayerLabel();
                case HierarchyItemDataType.Script:
                    return new HierarchyScriptLabel();
                case HierarchyItemDataType.TreeLines:
                    return new HierarchyTreeLinesLabel();
            }

            return null;
        }

        /// <summary>
        /// Does this label draw over the whole item?
        /// </summary>
        public virtual bool UsesWholeItemRect { get; } = false;

        /// <summary>
        /// Should this label draw for headers too?
        /// </summary>
        public virtual bool DrawForHeaders { get; } = false;

        #region Classes: Internal

        private class HierarchyIconLabel : HierarchyPropertyLabel
        {
            public override void OnGui(Rect rect)
            {
                var content = EditorGuiUtility.GetObjectContent(target, typeof(GameObject));
                if (content.image == null)
                {
                    return;
                }

                GUI.Label(rect, content.image);
            }
        }

        private class HierarchyToggleLabel : HierarchyPropertyLabel
        {
            private readonly GUIContent label = new GUIContent(string.Empty, "Enable/disable GameObject");

            public override void OnGui(Rect rect)
            {
                //NOTE: using EditorGUI.Toggle will cause bug and deselect all hierarchy toggles when you will pick a multi-selected property in the Inspector
                var result = GUI.Toggle(new Rect(rect.x + EditorGUIUtility.standardVerticalSpacing,
                        rect.y,
                        rect.width,
                        rect.height),
                    target.activeSelf, label);

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
            private const string untaggedTag = "Untagged";

            public override float GetWidth()
            {
                return Style.maxWidth;
            }

            public override void OnGui(Rect rect)
            {
                var content = new GUIContent(target.CompareTag(untaggedTag) ? string.Empty : target.tag, target.tag);
                EditorGUI.LabelField(rect, content, Style.defaultAlignTextStyle);
            }
        }

        private class HierarchyLayerLabel : HierarchyPropertyLabel
        {
            private GUIContent content;

            //NOTE: after replacing with SerializeReference-based implementation we should allow to pick if layer should be simplied (just number) or fully displayed
            private string GetContentText(LayerMask layerMask)
            {
                var layerName = LayerMask.LayerToName(layerMask);
                switch (layerMask)
                {
                    case 00: return string.Empty;
                    default: return layerName;
                }
            }

            public override bool Prepare(GameObject target, Rect availableRect)
            {
                if (!base.Prepare(target, availableRect))
                {
                    return false;
                }

                var layerMask = target.layer;
                var layerName = GetContentText(layerMask);
                content = new GUIContent(layerName);
                return true;
            }

            public override void OnGui(Rect rect)
            {
                EditorGUI.LabelField(rect, content, Style.centreAlignTextStyle);
            }

            public override float GetWidth()
            {
                if (string.IsNullOrEmpty(content.text))
                {
                    return base.GetWidth();
                }

                var size = Style.centreAlignTextStyle.CalcSize(content);
                return size.x + EditorGUIUtility.standardVerticalSpacing * 2;
            }
        }

        private class HierarchyScriptLabel : HierarchyPropertyLabel
        {
            private static Texture componentIcon;
            private static Texture transformIcon;
            private static Texture warningIcon;

            /// <summary>
            /// Cached components of the last prepared <see cref="target"/>.
            /// </summary>
            private Component[] components;
            private float baseWidth;
            private float summWidth;
            private bool isHighlighted;

            private GUIContent GetTooltipContent()
            {
                var componentsCount = components.Length;
                var tooltipBuilder = new StringBuilder();
                var tooltipContent = new GUIContent();

                tooltipBuilder.Append("Components:\n");
                for (var i = 1; i < componentsCount; i++)
                {
                    var component = components[i];
                    tooltipBuilder.Append("- ");
                    tooltipBuilder.Append(component != null ? component.GetType().Name : "<null>");
                    if (componentsCount - 1 != i)
                    {
                        tooltipBuilder.Append("\n");
                    }
                }

                tooltipContent.tooltip = tooltipBuilder.ToString();
                return tooltipContent;
            }

            private GUIContent GetContent(Component component)
            {
                if (component == null)
                {
                    return new GUIContent(image: warningIcon);
                }

                var content = EditorGUIUtility.ObjectContent(component, component.GetType());
                content.text = string.Empty;
                if (content.image == null)
                {
                    content.image = componentIcon;
                }

                return content;
            }

            private void CachePredefinedIcons()
            {
                componentIcon = componentIcon != null ? componentIcon : EditorGUIUtility.IconContent("cs Script Icon").image;
                transformIcon = transformIcon != null ? transformIcon : EditorGUIUtility.IconContent("Transform Icon").image;
                warningIcon = warningIcon != null ? warningIcon : EditorGUIUtility.IconContent("console.warnicon.sml").image;
            }

            public override bool Prepare(GameObject target, Rect availableRect)
            {
                var isValid = base.Prepare(target, availableRect);
                if (!isValid)
                {
                    return false;
                }

                baseWidth = Style.minWidth;
                components = target.GetComponents<Component>();
                var componentsCount = components.Length;
                summWidth = componentsCount > 1
                    ? (componentsCount - 1) * baseWidth
                    : baseWidth;

                isHighlighted = availableRect.Contains(Event.current.mousePosition);

                CachePredefinedIcons();
                return true;
            }

            public override float GetWidth()
            {
                return summWidth;
            }

            public override void OnGui(Rect rect)
            {
                var fullRect = rect;
                rect.xMin = rect.xMax - baseWidth;

                var componentsCount = components.Length;
                if (componentsCount <= 1)
                {
                    GUI.Label(fullRect, new GUIContent(transformIcon, "There is no additional component"));
                    return;
                }

                rect.xMin -= baseWidth * (componentsCount - 2);

                var iconRect = rect;
                iconRect.xMin = rect.xMin;
                iconRect.xMax = rect.xMin + baseWidth;

                //draw all icons associated to cached components (except transform)
                for (var i = 1; i < components.Length; i++)
                {
                    var component = components[i];
                    var content = GetContent(component);
                    //draw icon for the current component
                    GUI.Label(iconRect, content);
                    //adjust rect for the next script icon
                    iconRect.x += baseWidth;
                }

                if (isHighlighted)
                {
                    var tooltipContent = GetTooltipContent();
                    GUI.Label(fullRect, tooltipContent);
                }
            }
        }

        private class HierarchyTreeLinesLabel : HierarchyPropertyLabel, IDisposable
        {
            private const float firstElementWidthOffset = 4.0f;

#if UNITY_2019_1_OR_NEWER
            private const float firstElementXOffset = -45.0f;
            private const float startXPosition = 30.0f;
#else
            private const float firstElementXOffset = -15.0f;
            private const float startXPosition = 0.0f;
#endif
            private const float columnSize = 14.0f;

            private readonly List<TreeLineLevelRenderer> levelRenderers = new List<TreeLineLevelRenderer>();

            private int itemRenderCount = 0;

            public HierarchyTreeLinesLabel()
            {
                EditorApplication.update += ResetItemRenderCount;
            }

            public void Dispose()
            {
                EditorApplication.update -= ResetItemRenderCount;
            }

            public sealed override void OnGui(Rect rect)
            {
                if (Event.current.type != EventType.Repaint)
                {
                    return;
                }

                var levels = (int)((rect.x + firstElementXOffset) / columnSize);
                if (levels <= 0)
                {
                    return;
                }

                if (IsFirstRenderedElement)
                {
                    levelRenderers.Clear();
                }

                itemRenderCount++;

                rect.x = startXPosition;
                //we need 2x column size for full-line cases when object has no children and there is no foldout
                rect.width = 2 * columnSize + firstElementWidthOffset;

                var targetTransform = target.transform;
                var siblingIndex = targetTransform.GetSiblingIndex();

                if (levels > levelRenderers.Count)
                {
                    //initialize missing tree line level render
                    var startIndex = levelRenderers.Count;
                    int x;
                    for (x = startIndex; x < levels; x++)
                    {
                        var levelRenderer = new TreeLineLevelRenderer();
                        levelRenderers.Add(levelRenderer);
                    }

                    x--;

                    var transformBuffer = targetTransform;
                    for (; x >= startIndex; x--)
                    {
                        levelRenderers[x].Initialize(transformBuffer);
                        transformBuffer = transformBuffer.parent;
                    }
                }

                var colorCache = GUI.color;
                GUI.color = Color.gray;

                var i = 0;
                for (; i < (levels - 1); i++)
                {
                    levelRenderers[i].OnGUI(rect, target, siblingIndex, false);
                    rect.x += columnSize;
                }

                levelRenderers[i].OnGUI(rect, target, siblingIndex, true);

                GUI.color = colorCache;
            }

            private void ResetItemRenderCount()
            {
                itemRenderCount = 0;
            }

            public sealed override bool UsesWholeItemRect => true;

            public sealed override bool DrawForHeaders => true;

            private bool IsFirstRenderedElement => itemRenderCount == 0;

            private class TreeLineLevelRenderer
            {
                private bool renderedLastLevelGameobject = false;

                public void Initialize(Transform transform)
                {
                    var siblingIndex = transform.GetSiblingIndex();
                    renderedLastLevelGameobject = GetParentChildCount(transform) == (siblingIndex + 1);
                }

                public void OnGUI(Rect rect, GameObject target, int siblingIndex, bool isCurrentLevel)
                {
                    //NOTE: currently we are using labels and predefined chars to display tree lines, this is not optimal solution
                    // since we can't really control width, tickiness and other potential useful properties. Using few chars allow us
                    // to display dashed lines very easily but replacing it with standard line would a bit harder.
                    // For now this is ok solution but probably should be replaced with drawing lines using the EditorGUI.DrawRect API,
                    // in the same way we draw lines in the Inspector

                    if (isCurrentLevel)
                    {
                        var hasChildren = target.transform.childCount > 0;
                        GUIContent label;
                        if (GetParentChildCount(target) == (siblingIndex + 1))
                        {
                            renderedLastLevelGameobject = true;
                            label = hasChildren ? Style.treeElementLastHalf : Style.treeElementLast;
                        }
                        else
                        {
                            renderedLastLevelGameobject = false;
                            label = hasChildren ? Style.treeElementCrossHalf : Style.treeElementCross;
                        }

                        EditorGUI.LabelField(rect, label, Style.treeElementStyle);
                        return;
                    }

                    if (!renderedLastLevelGameobject)
                    {
                        EditorGUI.LabelField(rect, Style.treeElementPass, Style.treeElementStyle);
                    }
                }

                private int GetParentChildCount(GameObject gameObject)
                {
                    return GetParentChildCount(gameObject.transform);
                }

                private int GetParentChildCount(Transform transform)
                {
                    var parent = transform.parent;
                    if (parent != null)
                    {
                        return parent.childCount;
                    }

                    var scene = transform.gameObject.scene;
                    return scene.rootCount;
                }
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
            internal static readonly GUIStyle treeElementStyle;

            internal static readonly GUIContent treeElementLast;
            internal static readonly GUIContent treeElementLastHalf;
            internal static readonly GUIContent treeElementCross;
            internal static readonly GUIContent treeElementCrossHalf;
            internal static readonly GUIContent treeElementPass;

            internal static readonly Color characterColor;

            static Style()
            {
                treeElementLast = new GUIContent("└--");
                treeElementLastHalf = new GUIContent("└-");
                treeElementCross = new GUIContent("├--");
                treeElementCrossHalf = new GUIContent("├-");
                treeElementPass = new GUIContent("│");

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
                    alignment = TextAnchor.MiddleCenter
#else
                    fontSize = 8,
                    alignment = TextAnchor.UpperCenter
#endif
                };
                rightAlignTextStyle = new GUIStyle(EditorStyles.miniLabel)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 9,
                    alignment = TextAnchor.MiddleRight
#else
                    fontSize = 8,
                    alignment = TextAnchor.UpperRight
#endif
                };
                treeElementStyle = new GUIStyle(EditorStyles.label)
                {
                    padding = new RectOffset(4, 0, 0, 0),
                    fontSize = 12,
                };

                if (!EditorGUIUtility.isProSkin)
                {
                    treeElementStyle.normal.textColor = Color.white;
                }
            }
        }
    }
}