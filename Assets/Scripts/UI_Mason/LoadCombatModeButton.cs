using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadCombatModeButton : MonoBehaviour
{
    public event UnityAction OnLoadCombatModeClick;

    public void OnButtonClick()
    {
        if (OnLoadCombatModeClick != null) { OnLoadCombatModeClick.Invoke(); }
    }
}
