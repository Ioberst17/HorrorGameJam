using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStates
{
    public List<string> animationStates = new List<string>() 
    {
        // Movement 
        "PlayerIdle", "PlayerWalk", "PlayerRun", "PlayerDash",
        
        // Block
        "PlayerShield",
        
        // Crouching
        "PlayerStandToCrouch", "PlayerCrouch", "PlayerCrouchToStand",
        
        // Taking Damage
        "PlayerHurt", "PlayerDeath", "PlayerDead",
        
        // ATTACKS
            // Aerial
            "PlayerNeutralAir",
            "PlayerForwardAir",
        
            // Ground / Impact on Ground
            "PlayerCharge",
            "PlayerGroundSlam",
            "PlayerChargePunch",
            "PlayerBasicAttack1", "PlayerBasicAttack2", "PlayerBasicAttack3",
            "PlayerSideKick",
            "PlayerThrow",
        
        // Aerial Movement
        "PlayerJump", "PlayerFall", "PlayerWallLand", "PlayerLand"
    };
}
