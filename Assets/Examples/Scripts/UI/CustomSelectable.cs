using UnityEngine;
using UnityEngine.UI;

public class CustomSelectable : Selectable
{
    private interface ICustomLogic
    { }

    [SerializeReference]
    private ICustomLogic customLogic;
}