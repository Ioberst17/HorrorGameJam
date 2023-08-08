using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightArmAnimationEventSupport : MonoBehaviour
{
    public void ReleaseShot() { EventSystem.current.OnFireAnimationRelease(); } // right arm used on revolver
}
