# How to create custom Toolbox Drawers & Editors

## Custom Drawer

### Custom Decorator Drawer

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
	protected override void OnGuiBeginSafe(SampleAttribute attribute)
	{
		//draw something before property
	}
	
	protected override void OnGuiEndSafe(SampleAttribute attribute)
	{
		//draw something after property
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Label created in the custom decorator Drawer.");
	}
}
```

### Custom Condition Drawer

### Custom Self Property Drawer

### Custom List Property Drawer

### Custom Target Type Drawer

```csharp
using System;
using UnityEditor;
using UnityEngine;

public class IntDrawer : ToolboxTargetTypeDrawer
{
	public override void OnGui(SerializedProperty property, GUIContent label)
	{
		EditorGUILayout.LabelField("You can create a custom drawer for all single types");
		EditorGUILayout.PropertyField(property, label);
	}

	public override Type GetTargetType()
	{
		return typeof(int);
	}
	
	public override bool UseForChildren()
	{
		return false;
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