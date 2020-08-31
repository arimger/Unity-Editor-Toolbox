# How to create custom Toolbox Drawers & Editors

## Custom Drawer

```csharp
using UnityEngine;

public class SampleAttribute : ToolboxDecoratorAttribute
{ }

```

```csharp
using UnityEditor;
using Toolbox.Editor.Drawers;

public class SampleDrawer : ToolboxDecoratorDrawer<SampleAttribute>
{
    protected override void OnGuiEndSafe(SampleAttribute attribute)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Label created in the custom decorator Drawer.");
    }
}
```

## Custom Editor

```csharp
using UnityEditor;
using UnityEngine;
using Toolbox.Editor;

[CustomEditor(typeof(SampleBehaviour2))]
public class SampleEditor : ToolboxEditor
{
    private void OnEnable()
    { }

    private void OnDisable()
    { }

    public override void DrawCustomInspector()
    {
        base.DrawCustomInspector();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("This label is created in the custom Editor. You can freely extend Toolbox-based Editors by inheriting from the <b>ToolboxEditor</b> class.", Style.labelStyle);
    }


    private static class Style
    {
        internal static readonly GUIStyle labelStyle;

        static Style()
        {
            labelStyle = new GUIStyle(EditorStyles.helpBox)
            {
                richText = true,
                fontSize = 14
            };
        }
    }
}
```