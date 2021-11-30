# Unity Editor Toolbox

## Introduction

Improve usability and clarity of key features in Unity Editor for better workflow!

This Toolbox not only extends functionalities, it does so with the user in mind.
Written to be as flexible and optimized as possible. Now you and other programming professionals will be able to create a readable and useful component editor simply by using attributes. You’ll get fast and clear access to data from GameObjects placed in the Scene. Lastly, you’ll gain more control over the Project window. Go ahead, customize those folder icons.

It's worth to mention that prepared drawers are based on the custom, layout-based system. Additionally, I’m leaving you some useful scripts, classes, and functions that facilitate Editor extensions development.

Learn all the details about the main features below.

## System Requirements
Unity 2018.x or newer

## Dependencies

[EditorCoroutines](https://docs.unity3d.com/Packages/com.unity.editorcoroutines@0.0/api/Unity.EditorCoroutines.Editor.html)

## Installation

- Install Editor Toolbox package:
	- 1 way: Find Unity Package Manager (Window/Package Manager) and add package using this git URL:
	```https://github.com/arimger/Unity-Editor-Toolbox.git#upm```
	- 2 way: Copy and paste `Editor Toolbox` directory into your project (Assets/...) + add dependencies
- Open Edit/Project Settings/Editor Toolbox window
- If settings file is not found, press "Refresh" button or create new one
- Manage settings in your way
	- Enable/disable Hierarchy overlay, choose allowed information
	- Enable/disable Project icons or/and assign own directories
	- Enable/disable Toolbox drawers or/and assign custom drawers

## Table Of Contents

- [Attributes & Drawers](#drawers)
	- [Regular Drawers](#regulardrawers)
	- [Toolbox Drawers](#toolboxdrawers)
	- [Material Drawers](#materialdrawers)
- [Serialized Types](#serialized-types)
- [Editor Extensions](#editor-extensions)
	- [Hierarchy](#hierarchy)
	- [Project](#project)
	- [Toolbar](#toolbar)
	- [Utilities](#utilities)
- [Editor Extras](#editor-extras)

## Settings

The most important file, it allows the user to manage all available features. Can be accessed from the Project Settings window (Edit/Project Settings.../Editor Toolbox) or directly inside the Project window. Make sure to have one valid settings file per project.

Available features are divided into three groups:
- Hierarchy
- Project
- Inspector

Each module is described in its respective section.

## Attributes & Drawers <a name="drawers"></a>

### Regular Drawers <a name="regulardrawers"></a>

Drawers based on built-in classes **PropertyDrawer/DecoratorDrawer** and associated **PropertyAttribute**.

#### TagSelectorAttribute

```csharp
[TagSelector]
public string var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/tagselector.png)

#### ProgressBarAttribute

```csharp
[ProgressBar("Name", minValue: 0.0f, maxValue: 100.0f, HexColor = "#EB7D34")]
public float var1 = 80.0f;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/progressbar1.png)\
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/progressbar2.png)

#### NewLabelAttribute

```csharp
[NewLabel("Custom Label", "Element")]
public int[] vars1 = new int[3];
[NewLabel("Custom Label")]
public float var2 = 25.4f;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/newlabel.png)

#### MinMaxSliderAttribute

```csharp
[MinMaxSlider(0.5f, 71.7f)]
public Vector2 var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/minmaxslider.png)

#### AssetPreviewAttribute

```csharp
[AssetPreview]
public GameObject var1;
[AssetPreview(useLabel: false)]
public Component var2;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/assetpreview.png)

#### HideLabelAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/hidelabel.png)

#### SuffixAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/suffix.png)


#### EnumTogglesAttribute

```csharp
[System.Flags]
public enum FlagExample
{
	Nothing = 0,
	Flag1 = 1,
	Flag2 = 2,
	Flag3 = 4,
	Flag4 = 8,
	Flag5 = 16,
	Flag6 = 32,
	Flag7 = 64,
	Flag8 = 128,
	Flag9 = 256,
	Everything = ~0
}

[EnumToggles]
public FlagExample enumFlag = FlagExample.Flag1 | FlagExample.Flag2 | FlagExample.Flag6;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/enumtoggles.png)

#### NotNullAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/notnull1.png)\
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/notnull2.png)


#### DirectoryAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/directory1.png)\
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/directory2.png)

#### SceneNameAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/scenename1.png)\
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/scenename2.png)

#### PresetAttribute

```csharp
private readonly int[] presetValues = new[] { 1, 2, 3, 4, 5 };

[Preset(nameof(presetValues))]
public int presetTarget;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/preset.png)

#### SearchableEnumAttribute

```csharp
[SearchableEnum]
public KeyCode enumSearch;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/enumsearch.png)

#### ClampAttribute

```csharp
[Clamp(minValue = 1.5f, maxValue = 11.3f)]
public double var1;
```

#### PasswordAttribute

```csharp
[Password]
public string password;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/password.png)

#### LabelByChildAttribute

```csharp
[System.Serializable]
public class SampleClass1
{
	public Material var1;
	public KeyCode var2;
	public SampleClass2 var3;
}

[System.Serializable]
public class SampleClass2
{
	public int var1;
	public string var2;
}

[LabelByChild("var2")]
public SampleClass1[] vars1;
[LabelByChild("var3.var2")]
public SampleClass1 var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/labelbychild1.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/labelbychild2.png)

#### ChildObjectOnlyAttribute

#### SceneObjectOnlyAttribute

#### PrefabObjectOnlyAttribute

#### LeftToggleAttribute

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/lefttoggle.png)

#### FormattedNumberAttribute

```csharp
[FormattedNumber]
public int bigNumber;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/formattednumber.png)

---

### Toolbox Drawers <a name="toolboxdrawers"></a>

Drawers are based on classes inherited from the **ToolboxDrawer** class and associated **ToolboxAttribute**. With this powerful custom system you are able to create really flexible drawers. You can use them without limitations (they work with sub-classes and as array children). Every ToolboxDrawer is layout-based. For proper work they need at least one settings file located in your project. You can find predefined one here - `Editor Toolbox/EditorSettings.asset`.

Examples **'How to'** create custom ToolboxDrawers you can find [HERE](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Assets/Editor%20Toolbox/HOWTO.md).

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/inspector.png)

#### ToolboxDecoratorAttributes

Display/create something before and after property in the desired order (using Order property).   
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
[BeginHorizontal(labelToWidthRatio: 0.1f)]
public int var1;
public int var2;
[EndHorizontal]
public int var3;

[BeginHorizontalGroup(label: "Horizontal Group")]
public GameObject gameObject;
[SpaceArea]
[EndHorizontalGroup]
[ReorderableList]
public int[] ints;
```
```csharp
[BeginIndent]
public int var1;
public int var2;
public int var3;
[EndIndent]
public int var4;

[IndentArea(4)]
public int var5;
```
```csharp
[SpaceArea(spaceBefore = 10.0f, spaceAfter = 5.0f, Order = 1)]
public int var1;
```
```csharp
[Label("My Custom Header", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
public int var1;
```
```csharp
[EditorButton(nameof(MyMethod), "<b>My</b> Custom Label", activityType: ButtonActivityType.OnPlayMode)]
public int var1;

private void MyMethod()
{
	Debug.Log("MyMethod is invoked");
}
```
```csharp
[Highlight(0, 1, 0)]
public int var1;
```
```csharp
[Help("Help information", UnityMessageType.Warning, Order = -1)]
public int var1;
[DynamicHelp(nameof(Message), UnityMessageType.Error)]
public int var2;

public string Message => "Dynamic Message";
```
```csharp
[ImageArea("https://img.itch.zone/aW1nLzE5Mjc3NzUucG5n/original/Viawjm.png", 150.0f)]
public int var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/decorators.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/horizontal.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/imagearea.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/helpbox.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/button.png)

#### ToolboxConditionAttributes 

Enable/disable or show/hide properties using custom conditions. You can use them together with any other type of drawer.
Every ToolboxConditionDrawer supports boolean, int, string, UnityEngine.Object and enum types and works even with array/list properties.
You are able to pass values from fields, properties, and methods.

```csharp
public string StringValue => "Sho";
[ShowIf(nameof(StringValue), "show")]
public int var1;

public GameObject objectValue;
[HideIf(nameof(objectValue), false)]
public int var2;
```

```csharp
public KeyCode enumValue = KeyCode.A;
[EnableIf(nameof(enumValue), KeyCode.A)]
public int var1;

[DisableIf(nameof(GetFloatValue), 2.0f, Comparison = UnityComparisonMethod.GreaterEqual)]
public int var2;

public float GetFloatValue()
{
	return 1.6f;
}
```

```csharp
[DisableInPlayMode]
public int var1;
```

```csharp
public bool boolValue = true;
[ShowWarningIf(nameof(boolValue), false, "Message", DisableField = true)]
public int var1;
```

```csharp
[Disable, ReorderableList]
public int[] vars1 = new [] { 1, 2, 3, 4 };
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/disabled.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/enableif1.png)\
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/enableif2.png)

#### InLineEditorAttribute

This attribute gives a great possibility to extend all reference-related (UnityEngine.Object) fields. 
Using it you are able to 'inline' Editors for: components, ScriptableObjects, Materials, Renderers, MeshFilters, Textures, AudioClips, etc.

```csharp
[InLineEditor(DisableEditor = false)]
public Transform var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/inlined3.png)
```csharp
[InLineEditor]
public AudioClip var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/inlined2.png)
```csharp
[InLineEditor(true, true)]
public Material var1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/inlined1.png)

#### Reorderable List

Custom implementation of standard ReorderableList (UnityEditorInternal). Usable as an attribute in serialized fields or a single object in custom Editors.

```csharp
var list = new ReorderableList(SerializedProperty property, string elementLabel, bool draggable, bool hasHeader, bool fixedSize);
```
```csharp
[ReorderableList, InLineEditor]
public Canvas[] vars1;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/list4.png)
```csharp
[ReorderableList(ListStyle.Lined, "Item", Foldable = false)]
public List<int> linedStyleList;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/list1.png)
```csharp
[ReorderableList(ListStyle.Round)]
public List<string> standardStyleList;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/list2.png)
```csharp
[ReorderableList(ListStyle.Boxed, fixedSize: true)]
public GameObject[] boxedStyleList = new GameObject[4];
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/list3.png)


```csharp
[ReorderableListExposed(OverrideNewElementMethodName = nameof(GetValue))]
public int[] list;

private int GetValue()
{
	return list.Length + Random.Range(0, 4);
}
```

#### ScrollableItemsAttribute

It's a perfect solution to inspect large arrays/lists and optimize displaying them within the Inspector window.

```csharp
[ScrollableItems(defaultMinIndex: 0, defaultMaxIndex: 5)]
public GameObject[] largeArray = new GameObject[19];
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/scrollableitems.png)

#### Other ToolboxProperty attributes

```csharp
[IgnoreParent]
public Quaternion q;
```

```csharp
[DynamicMinMaxSlider(nameof(minValue), nameof(MaxValue))]
public Vector2 vec2;

public float minValue;
public float MaxValue => 15.0f;
```

#### ToolboxArchetypeAttributes

Using this attribute you are able to implement custom patterns of frequently grouped **ToolboxAttributes**.


```csharp
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class TitleAttribute : ToolboxArchetypeAttribute
{
	public TitleAttribute(string label)
	{
		Label = label;
	}


	public override ToolboxAttribute[] Process()
	{
		return new ToolboxAttribute[]
		{
			new LabelAttribute(Label),
			new LineAttribute(padding: 0)
			{
				ApplyIndent = true
			}
		};
	}


	public string Label { get; private set; }
}
```

```csharp
[Title("Header")]
public int var1;
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/title.png)

### Material Drawers <a name="materialdrawers"></a>

```
[CompactTexture]
_MainTex ("Texture", 2D) = "white" {}
[Vector2]
_Vector1 ("Vector2", Vector) = (0.0, 0.0, 0.0)
[Vector3]
_Vector2 ("Vector3", Vector) = (0.0, 0.0, 0.0)
[MinMaxSlider(20.0, 165.0)]
_Vector3 ("MinMax Vector", Vector) = (50.0, 55.0, 0.0)
[Indent(3)]
_Float1 ("Float1", Float) = 0.0
[Help(Custom Help Box , 1)]
_Float2 ("Float2", Float) = 0.0
_Float3 ("Float3", Float) = 0.0
[Title(Custom Title, 4)]
_Float ("Float", Float) = 0.5
[Toggle][Space]
_ToggleProperty ("Toggle", Int) = 0
[ShowIfToggle(_ToggleProperty)]
_ShowIfExample ("Texture", 2D) = "White" {}
[HideIfToggle(_ToggleProperty)]
_HideIfExample ("Range", Range(0, 1)) = 0.75
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/customshader.png)

## Serialized Types

#### SerializedType

Allows to serialize Types and pick them through a dedicated picker.

```csharp
[ClassExtends(typeof(Collider), Grouping = ClassGrouping.None, AddTextSearchField = false)] //or [ClassImplements(typeof(interface))] for interfaces
public SerializedType var1;

public void Usage()
{
	var type = var1.Type;
}
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/serializedtype.png)

#### SerializedScene

Allows to serialize SceneAssets and use them in Runtime.

```csharp
public SerializedScene scene;

public void Usage()
{
	UnityEngine.SceneManagement.SceneManager.LoadScene(scene.BuildIndex);
}
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/serializedscene.png)

```csharp
[SceneDetails]
public SerializedScene scene;
```
![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/scenedetails.png)

#### SerializedDictionary<TK, TV>

Allows to serialize and use Dictionaries. The presented class implements the IDictionary interface, so it can be easily used like the standard version.

Requires at least Unity 2020.1.x because of generic serialization and has to be assigned in the Settings file.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/dictionary3.png)

```csharp
#if UNITY_2020_1_OR_NEWER
public SerializedDictionary<int, GameObject> dictionary;

public void Usage()
{
	dictionary.Add(3, new GameObject("TestObject"));
	dictionary.ContainsKey(2);
	//etc. like standard System.Collections.Generic.Dictionary<>
	var nativeDictionary = dictionary.BuildNativeDictionary();
}
#endif
```

#### SerializedDateTime

Allows to serialize DateTime.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/dictionary1.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/dictionary2.png)

## Editor Extensions

### Hierarchy <a name="hierarchy"></a>

Enable and customize the presented hierarchy overlay in the **ToolboxEditorSettings**. Basically it provides more data about particular GameObjects directly within the Hierarchy window. Additionally, you can create special 'Header' objects using the '#h' prefix or Create menu: **GameObject/Editor Toolbox/Hierarchy Header** (by default created object will have **EditorOnly** tag).

Each row can contain:
- Scripts information
- Layer
- Tag
- Toggle to enable/disable GameObject
- Icon

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/hierarchy.png)

### Project <a name="project"></a>

Set custom folder icons in the **ToolboxEditorSettings**.

Properties that can be edited include:
- XY position and scale of the large icon
- XY position and scale of the small icon
- Path to directory or name (depends on picked item type)
- Optional tooltip
- Large icon
- Small icon

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/project1.png)

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/project2.png)

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
		ToolboxEditorToolbar.OnToolbarGui += OnToolbarGui;
	}
	
	private static void OnToolbarGui()
	{
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("1", Style.commandLeftStyle))
		{
			Debug.Log("1");
		}
		if (GUILayout.Button("2", Style.commandMidStyle))
		{
			Debug.Log("2");
		}
		if (GUILayout.Button("3", Style.commandMidStyle))
		{
			Debug.Log("3");
		}
		if (GUILayout.Button("4", Style.commandMidStyle))
		{
			Debug.Log("4");
		}
		if (GUILayout.Button("5", Style.commandRightStyle))
		{
			Debug.Log("5");
		}
	}
}
```

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/toolbar.png)

### Utilities <a name="utilities"></a>

Copy and paste all components from/to particular GameObject.

![inspector](https://github.com/arimger/Unity-Editor-Toolbox/blob/develop/Docs/utils.png)

## Editor Extras

I decieded to move additional editors and tools to Gist.

### Prefab/GameObject Painter

Check it out [HERE](https://gist.github.com/arimger/00842a217ea8ab03d4e1b81f11592cf3).
