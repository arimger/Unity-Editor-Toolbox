# How to create custom Toolbox Drawers & Editors

## Custom Drawer

ToolboxDrawers are divided into 5 sub-types to maximize useability and cover all popular cases.
- **ToolboxDecoratorDrawers:**

	Use them to create nice looking controls, icons and optional 'decorators' before or/and after any serialized property.<br/> 
	There is no limit of usages within single property.
- **ToolboxConditionDrawers:**

	Use them to validate the state of any property (even lists), you can easily decide if a property should be disabled or hidden.<br/> 
	Use it only once per property.
- **ToolboxSelfPropertyDrawers:**

	Use them to customize the look of the respective property field.<br/>
	Use it only once per property.
- **ToolboxListPropertyDrawers:**

	Use them to customize the look of the respective list property.<br/> 
	Use it only once per property.
- **ToolboxTargetTypeDrawers:**

	Use them to specify the look of all dedicated properties based on the chosen type.
	
**Implementation steps:**
- decide what kind of drawer you need
- implement dedicated ToolboxAttribute class (if drawer is attribute-based)
- implement dedicated ToolboxDrawer class
- assign newly created drawer in the featured Settings file
- use it 

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
	
	protected override void OnGuiCloseSafe(SampleAttribute attribute)
	{
		//draw something after property
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Label created in the custom decorator Drawer.");
	}
}
```

### Custom Condition Drawer

```csharp
using UnityEngine;

public class SampleAttribute : ToolboxConditionAttribute
{ }
```

```csharp
using UnityEditor;
using Toolbox.Editor.Drawers;

public class SampleDrawer : ToolboxConditionDrawer<SampleAttribute>
{
	protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, SampleAttribute attribute)
	{
		return PropertyCondition.Disabled;
	}
}
```

### Custom Self Property Drawer

```csharp
using UnityEngine;

public class HideLabelAttribute : ToolboxSelfPropertyAttribute
{
	//since we want only to hide the label there is no additional data needed
}

```

```csharp
using UnityEditor;
using UnityEngine;
using Toolbox.Editor.Drawers;

public class HideLabelAttributeDrawer : ToolboxSelfPropertyDrawer<HideLabelAttribute>
{
	protected override void OnGuiSafe(SerializedProperty property, GUIContent label, HideLabelAttribute attribute)
	{
		//hide label and draw property in the standard way
		EditorGUILayout.PropertyField(property, GUIContent.none, property.isExpanded);
	}


	public override bool IsPropertyValid(SerializedProperty property)
	{
		//drawer is valid for all properties (for all types)
		return true;
	}
}
```

### Custom List Property Drawer

```csharp
using UnityEngine;

public class StandardListAttribute : ToolboxListPropertyAttribute
{ }
```

```csharp
using UnityEditor;
using UnityEngine;
using Toolbox.Editor.Drawers;

public class StandardListAttributeDrawer : ToolboxListPropertyDrawer<StandardListAttribute>
{
	protected override void OnGuiSafe(SerializedProperty property, GUIContent label, StandardListAttribute attribute)
	{
		EditorGUILayout.LabelField("Custom list drawer example");
		EditorGUILayout.PropertyField(property, label, false);
		if (property.isExpanded)
		{
			EditorGUILayout.PropertyField(property.FindPropertyRelative("Array.size"));
			var size = property.arraySize;
			for (var i = 0; i < size; i++)
			{
				var element = property.GetArrayElementAtIndex(i);
				EditorGUILayout.PropertyField(element, element.isExpanded);
			}
		}
	}
}
```

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
#if UNITY_2019_1_OR_NEWER
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif
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
		
		//for custom properties:
		// - ToolboxEditorGui.DrawToolboxProperty(serializedObject.FindProperty("myProperty"));
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("This label is created in the custom Editor. You can freely extend Toolbox-based Editors by inheriting from the <b>ToolboxEditor</b> class.", Style.labelStyle);
	}
	
#if UNITY_2019_1_OR_NEWER
	public override VisualElement CreateInspectorGUI()
	{
		//NOTE: create a custom container if you want to use UIElements instead of the standard IMGUI system
		return null;
	}
#endif

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
