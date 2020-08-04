# Unity Editor Toolbox

## Introduction

Provided here Toolbox is focused on additional Editor features. The main reason for its creation was to improve the useability and clarity of Unity Editor's key functionalities. The most time-consuming part was the preparation of custom property drawers and whole proprietary, layout-based drawers system. It allows programmers to create a readable and useful component editor only by using attributes. Next crucial thing is fast and clear access to the data from particular Game Objects placed in the Scene. Therefore it was decided to extend the hierarchy with additional information. Last main module of presented Toolbox is focused on the Project window. The provided extension allows user to customize folder icons. Additionally, this repository contains many useful scripts, classes, and functions for Editor extensions development. It's worth to mention this code was written to be as flexible and optimized as possible. More information about the main features is described in the following sections.

## System Requirements
Unity 2018.x or newer

## Installation

- Copy and paste `Editor Toolbox` directory into your project (basically into `Asset` directory or somewhere deeper)
- Open Edit/Project Settings/Editor Toolbox window
- If Toolbox Editor Settings is not available, press "Try to find the settings file" button or create new
- Manage settings in your way
	- Enable/disable Hierarchy overlay
	- Enable/disable Project icons or/and assign own directories
	- Enable/disable Toolbox drawers or/and assign custom drawers

## Table Of Contents

- [Attributes](#attributes)
	- [Native Drawers](#nativedrawers)
	- [Toolbox Drawers](#toolboxdrawers)
- [ReorderableList](#reorderable-list)
- [Editor Extensions](#editor-extensions)
	- [Hierarchy](#hierarchy)
	- [Project](#project)
	- [Toolbar](#toolbar)
- [Editor Extras](#editor-extras)

## Settings

The most important file, it allows user to manage all available features. Can be accessed within the Project Settings window (Edit/Project Settings.../Editor Toolbox) or directly from the Project window. Make sure to have one valid settings file within a project.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc14.png)

Available features are divided into three sections:
- Hierarchy
- Project
- Inspector 

Each module is described in details within the associated section.

## Attributes

### Native Drawers <a name="nativedrawers"></a>

Drawers based on build-in classes **PropertyDrawer/DecoratorDrawer** and associated **PropertyAttribute**.

&nbsp;

> Editor Toolbox/Scripts/Attributes/\
> Editor Toolbox/Editor/Drawers/

&nbsp;

#### HelpAttribute

```csharp
[Help("You can provide more information in HelpBoxes.", order = 100)]
public int var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc1.png)

#### TagSelectorAttribute

```csharp
[TagSelector]
public string var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc2.png)

#### SeparatorAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc3.png)

#### ProgressBarAttribute

```csharp
[ProgressBar(minValue:0.0f, maxValue:100.0f)]
public float var1 = 36.0f;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc4.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc5.png)

#### NewLabelAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc12.png)

#### MinMaxSliderAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc6.png)

#### IndentAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc7.png)

#### ConditionalHideAttribute

```csharp
public bool toggle;
[ConditionalHide(nameof(toggle), true)]
public float var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc9.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc8.png)

#### ConditionalDisableAttribute

```csharp
public bool toggle;
[ConditionalDisable(nameof(toggle), true)]
public float var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc24.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc25.png)

#### AssetPreviewAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc10.png)

#### HideLabelAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc11.png)

#### SuffixAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc13.png)

#### TypeConstraintAttribute

```csharp
[ClassExtends(typeof(UnityEngine.Object))]
public SerializedType type1;
[ClassImplements(typeof(System.Collections.ICollection))]
public SerializedType type2;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc14.png)

#### ReadOnlyFieldAttribute

```csharp
[ReadOnlyField]
public int var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc15.png)

#### BoxedHeaderAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc16.png)

#### BoxedToggleAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc17.png)

#### EnumFlagAttribute

```csharp
[System.Flags]
public enum FlagExample
{
    Nothing = 0,
    Flag1 = 1,
    Flag2 = 2,
    Flag3 = 4,
    Everything = ~0
}

[EnumFlag]
public FlagExample enumFlag = FlagExample.Flag1 | FlagExample.Flag2;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc20.png)

```csharp
[EnumFlag(EnumStyle.Button)]
public FlagExample enumFlag = FlagExample.Flag1 | FlagExample.Flag2 | FlagExample.Flag4 | FlagExample.Flag8;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc35.png)

#### NotNullAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc21.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc22.png)

#### RandomAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc23.png)

#### DirectoryAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc26.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc27.png)

#### BroadcastButtonAttribute

```csharp
//NOTE1: to broadcast messages in Edit mode desired component has to have [ExecuteAlways] or [ExecuteInEditMode] attribute
//NOTE2: Unity broadcasting will invoke all matching methods on this behaviour

[BroadcastButton(nameof(MyMethod), "Click me to broadcast message", ButtonActivityType.OnEditMode, order = 100)]
public int var1;

private void MyMethod()
{
	Debug.Log("MyMethod is invoked");
}
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc28.png)

#### InstanceButtonAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc29.png)

#### SceneNameAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc30.png)
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc31.png)

#### PresetAttribute

```csharp
private readonly int[] presetValues = new[] { 1, 2, 3, 4, 5 };

[Preset("presetValues")]
public int presetTarget;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc33.png)

#### SearchableEnumAttribute

```csharp
[SearchableEnum]
public KeyCode enumSearch;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc34.png)

#### ClampAttribute

```csharp
[Clamp(minValue = 1.5f, maxValue = 11.3f)]
public double var1;
```

---

### Toolbox Drawers <a name="toolboxdrawers"></a>

Drawers based on classes inheriting from **ToolboxDrawer** and associated **ToolboxAttribute**. A quite powerful custom system that allows you to create really flexible drawers. You can use them without limitations (they work with sub-classes and as array children). Every ToolboxDrawer is layout-based. For proper work they need at least one settings file located in your project. You can find predefined one here - `Editor Toolbox/EditorSettings.asset`.

&nbsp;

> Editor Toolbox/Scripts/Attributes/ToolboxAttributes\
> Editor Toolbox/Editor/Drawers/ToolboxDrawers

&nbsp;

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc5.png)

#### ToolboxDecoratorAttributes

Display/create something before and after property in the desired order(using Order property).   
In fact **ToolboxDecoratorDrawers** are like extended version of built-in **DecoratorDrawers**. 
Unfortunately, standard decorators won't always work with ToolboxDrawers so try to use this replacement instead.

```csharp
[BeginGroup("Group1")]
public int var1;
public int var2;
public int var3;
[EndGroup]
public int var4;
```
```csharp
[BeginHorizontal]
public int var1;
public int var2;
[EndHorizontal]
public int var3;
```
```csharp
[BeginIndent]
public int var1;
public int var2;
public int var3;
[EndIndent]
public int var4;
```
```csharp
[SpaceArea(spaceBefore = 10.0f, spaceAfter = 5.0f, Order = 1)]
public int var1;
```
```csharp
[HeaderArea("My Custom Header")]
public int var1;
```
```csharp
[Highlight(0, 1, 0)]
public int var1;
```

#### ReorderableListAttribute

```csharp
[ReorderableList(ListStyle.Round)]
public List<string> standardStyleList;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc7.png)

#### InLineEditorAttribute

```csharp
[InLineEditor]
public Transform var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc32.png)
```csharp
[InLineEditor]
public AudioClip var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc36.png)
```csharp
[InLineEditor(drawHeader:false, drawPreview:true)]
public Material var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc37.png)

#### HideAttribute

Hides any property.

#### HideIfAttribute

Same like standard PropertyDrawer for **ConditionalHideAttribute** but works with Enum types and arrays/lists.   
Can be used additionally to any **PropertyDrawer** or **ToolboxPropertyDrawer**.

#### DisableAttribute

Disables any property.   
Can be used additionally to any **PropertyDrawer** or **ToolboxPropertyDrawer**.  

#### DisableIfAttribute

Same as standard PropertyDrawer for **ConditionalDisableAttribute** but works with Enum types and arrays/lists.   
Can be used additionally to any **PropertyDrawer** or **ToolboxPropertyDrawer**.

```csharp
[Disable, ReorderableList]
public int[] vars1 = new [] { 1, 2, 3, 4 };
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/Attributes/doc19.png)

## Reorderable List

Custom implementation of standard ReorderableList(UnityEditorInternal). Useable as an attribute in inspector fields or a single object in custom editors.

> Editor Toolbox/Editor/Internal/ReorderableList.cs

```csharp
var list = new ReorderableList(SerializedProperty property, string elementLabel, bool draggable, bool hasHeader, bool fixedSize);
```
```csharp
[ReorderableList(ListStyle.Lined, "Item")]
public List<int> linedStyleList;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc6.png)
```csharp
[ReorderableList(ListStyle.Round)]
public List<string> standardStyleList;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc7.png)
```csharp
[ReorderableList(ListStyle.Boxed, fixedSize: true)]
public GameObject[] boxedStyleList = new GameObject[4];
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc8.png)

## Editor Extensions

### Hierarchy <a name="hierarchy"></a>

Enable custom hierarchy overlay in **ToolboxEditorSettings**. Basically it provides more data about particular GameObjects directly within the Hierarchy window. 

Each row contains:
- Layer
- Tag
- Toggle to enable/disable GameObject
- Icon

> Editor Toolbox/Editor/ToolboxEditorHierarchy.cs

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc2.png)

### Project <a name="project"></a>

Set custom folder icons in **ToolboxEditorSettings**.

Properties that can be edited include:
- XY position and scale of the large icon
- XY position and scale of the small icon
- Path to directory or name (depends on picked item type)
- Optional tooltip
- Large icon
- Small icon

> Editor Toolbox/Editor/ToolboxEditorProject.cs

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc12.png)  

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc11.png)

### Toolbar <a name="toolbar"></a>

> Editor Toolbox/Editor/ToolboxEditorToolbar.cs

Check **Examples** for more details.

> Examples/Editor/SampleToolbar.cs

```csharp
using Toolbox.Editor;

[UnityEditor.InitializeOnLoad]
public static class MyEditorUtility
{
    static MyEditorUtility()
    {
        ToolboxEditorToolbar.AddToolbarButton(new ToolbarButton(() => Debug.Log("1"), new GUIContent("1")));
        ToolboxEditorToolbar.AddToolbarButton(new ToolbarButton(() => Debug.Log("2"), new GUIContent("2")));
        ToolboxEditorToolbar.AddToolbarButton(new ToolbarButton(() => Debug.Log("3"), new GUIContent("3")));
        ToolboxEditorToolbar.AddToolbarButton(new ToolbarButton(() => Debug.Log("4"), new GUIContent("4")));
        ToolboxEditorToolbar.AddToolbarButton(new ToolbarButton(() => Debug.Log("5"), new GUIContent("5")));
    }
}
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc10.png)

### Utilities 

Copy and paste all components from/to particular GameObject.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Documentation/doc13.png)

## Editor Extras

I decieded to move additional editors and tools to Gist.

### Prefab/GameObject Painter

Check it out [HERE](https://gist.github.com/arimger/00842a217ea8ab03d4e1b81f11592cf3)

---

### Field of View Generator

Check it out [HERE](https://gist.github.com/arimger/f4ce98e9d3c5b0d4fb33a92e648f8442)

### Grid Generator

Check it out [HERE](https://gist.github.com/arimger/4c955744bb2e65f56e82117eed2e78cc)
