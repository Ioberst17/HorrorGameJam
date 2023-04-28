using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadGameButton : MonoBehaviour
{
    public event UnityAction<int> OnLoadGameButtonClick;

    public void OnButtonClick(int fileNumber)
    {
        if (OnLoadGameButtonClick != null) { OnLoadGameButtonClick.Invoke(fileNumber); }
    }
}
