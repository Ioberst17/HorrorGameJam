using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRightArmAnimationEventSupport : MonoBehaviour
{
    public void ReleaseShot() { EventSystem.current.OnFireAnimationRelease(); } // right arm used on revolver
}
