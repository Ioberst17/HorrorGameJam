using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A list of animation states that must be updated as player animations are added. 
/// It is used to build an animation state dictionary that the player object uses.
/// </summary>
public class PlayerAnimationStates
{
    public List<string> animationStates = new List<string>() 
    {
        // Movement 
        "PlayerIdle", "PlayerWalk", "PlayerRun", "PlayerDash",

        // Aerial Movement
        "PlayerJump", "PlayerFall", "PlayerWallLand", "PlayerLand",
        
        // Idles
        "PlayerMeditate", "PlayerStandToMeditate",

        // Block
        "PlayerShield",
        
        // Crouching
        "PlayerStandToCrouch", "PlayerCrouch", "PlayerCrouchToStand", "PlayerCrouchDodge",
        
        // Crouch Attacks
        "PlayerNeutralCrouchAttack",

        // Taking Damage
        "PlayerHurt", "PlayerDeath", "PlayerDead",
        
        // ATTACKS (non-Crouch)
            // Aerial
            "PlayerNeutralAir",
            "PlayerForwardAir",
        
            // Ground / Impact on Ground / Crouching
            "PlayerCharge",
            "PlayerGroundSlam",
            "PlayerChargePunch",
            "PlayerBasicAttack1", "PlayerBasicAttack2", "PlayerBasicAttack3",
            "PlayerSideKick",
            "PlayerSideKnee",
            "PlayerThrow",

        // SHOOTING
        "PlayerShoot", "PlayerShootReturn"
    };
}
