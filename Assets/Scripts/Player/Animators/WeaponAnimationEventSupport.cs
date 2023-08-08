using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationEventSupport : MonoBehaviour
{
    public void ReleaseShot() { EventSystem.current.OnFireAnimationRelease(); } // weapon used on hunter's rifle
}
