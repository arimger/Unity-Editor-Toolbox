using UnityEngine;
using UnityEngine.UI;

public class CustomSelectable : Selectable
{
    private interface ICustomLogic
    { }

#if UNITY_2019_3_OR_NEWER
    [SerializeReference]
    private ICustomLogic customLogic;
#endif
}