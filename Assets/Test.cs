using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [ReorderableList, DisableIf(Order = 0)]
    public List<int> list;

    [DisableIf]
    public int i;
}