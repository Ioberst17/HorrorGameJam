using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using System.Linq;
using static ComponentFinder;

[RequireComponent(typeof(PlayerAttackManager))]
public class ChargePunch : MonoBehaviour
{
    // external references
    GameController gameController;
    CameraBehavior cameraBehavior;
    PlayerAttackManager playerAttackManager;
    PlayerVisualEffectsController playerVisualEffectsController;
    AudioManager audioManager;

    // internal variables
    [SerializeField] float maxChargeTime = 2f;      // the maximum time the punch can be charged for
    [SerializeField] float chargeSpeed = 1f;        // the speed at which the punch charge increases
    [SerializeField] float minChargeTime = 0.35f; // minimum time before charge counts
    int BaseDamage {get; set; }
    int DamageToPass { get; set; }
    [SerializeField] float holdTimeNormalized;

    [SerializeField] float chargeTime; // the current charge time
    [SerializeField] bool _isCharging; public bool IsCharging { get { return _isCharging; } set { _isCharging = value; } } // whether the punch is currently being charged


    // MODIFY COLLIDERS BASED ON CHARGE LENGTH, update based on the size of the charge punch sprite
    [SerializeField] Vector3 _upperRightCorner; public Vector3 UpperRightCorner { get { return _upperRightCorner; } set { _upperRightCorner = value; } }
    [SerializeField] Vector3 _bottomLeftCorner; public Vector3 BottomLeftCorner { get { return _bottomLeftCorner; } set { _bottomLeftCorner = value; } }


    [Header("VFX Related")]
    // these should match the names of the game objects in the VisualEffectsController
    List<string> FirstWaveParticleSystems = new List<string>
    {
            "PunchWindParticlesTop1",
            "PunchWindParticlesTop2",
            "PunchWindParticlesTop3",
            "PunchWindParticlesFloor1",
            "PunchWindParticlesFloor2"
    };
    List<string> SecondWaveParticleSystems = new List<string>
        {
            "ChargePunchInitialBlast",
            "ChargePunchInitialElectricity",
            "ChargePunchInitialElectricity2", 
        };    
    
    List<string> ThirdWaveParticleSystems = new List<string> { "ChargePunchFinalElectricity", "ChargePunchFireSpiral" };

    // add together particle systems above to get a full list at runtime
    List<string> AllParticleSystems = new List<string> { };

    // tracked to ensure particle systems don't multitrigger
    bool particleTrigger;

    [SerializeField] float firstWaveParticleSystemsTimeDelay; // used to match electricity start and SFX loop

    // charge punch sprite related
    public string PunchSprite = "ChargePunchSprite";
    float maxScale = 2.0f; // SETS MAX SCALE OF SPRITE IN CHARGING, IMPORTANT FOR COLLISIONS
    float offsetAmount = 1.0f;
    float fadeDuration = 0.5f;

    //SFX Related
    bool falconSFXFlag, falconSFXPlaying;
    float punch1Length, sfxPlayTime, additionalSFXWaitTime;

    private void Start()
    {
        GetSupportingComponents();
        LoadVFXReferences();
        LoadAudioReferences();
        EventSystem.current.onChargePunchRelease += ReleasePunchOnFrame;
        EventSystem.current.onPlayerHitPostHealthTrigger += ReleaseChargeIfHit;
    }

    private void OnDestroy() 
    { 
        EventSystem.current.onChargePunchRelease -= ReleasePunchOnFrame;
        EventSystem.current.onPlayerHitPostHealthTrigger -= ReleaseChargeIfHit;
    }

    void GetSupportingComponents()
    {
        playerAttackManager = GetComponent<PlayerAttackManager>();
        BaseDamage = 10;
        gameController = FindObjectOfType<GameController>();
    }

    void LoadVFXReferences()
    {
        playerVisualEffectsController = SiblingComponentUtils.GetSiblingComponent<PlayerVisualEffectsController>(gameObject);

        // screen shake related to build particles blast
        cameraBehavior = FindObjectOfType<CameraBehavior>();

        AllParticleSystems.AddRange(FirstWaveParticleSystems);
        AllParticleSystems.AddRange(SecondWaveParticleSystems);
        AllParticleSystems.AddRange(ThirdWaveParticleSystems);

        ParticleSystemsOn(false);
    }

    void LoadAudioReferences()
    {
        audioManager = FindObjectOfType<AudioManager>();
        Sound punch1Sound = audioManager.GetSFX("ChargePunch1");
        punch1Length = punch1Sound.clip.length;
    }

    public void Execute() { IsCharging = true; }

    public void Release(int attackDirection) {  if (IsCharging) { playerAttackManager.StartAttack(attackDirection, "PlayerChargePunch"); } } 

    void FixedUpdate()
    {
        if (IsCharging)
        {
            chargeTime = chargeTime + (Time.deltaTime * chargeSpeed);
            CalcForce();
            HandleSpriteSize();
            HandleChargeVFX();
            HandleChargeSound();
            playerVisualEffectsController.HandleGlow();
        }
        if (falconSFXPlaying) { sfxPlayTime += Time.deltaTime; }
    }

    private void CalcForce()
    {
        holdTimeNormalized = Mathf.Clamp01(chargeTime / maxChargeTime);
        if(playerAttackManager.MostRecentAttack.name != "Charge Punch")
        {
            DamageToPass = (int)(holdTimeNormalized * BaseDamage);
        }
    }

    private void HandleSpriteSize() // scales with charge punch time
    {
        float scale = Mathf.Lerp(1.0f, maxScale, chargeTime / maxChargeTime);
        playerVisualEffectsController.SetSpriteLocalScale(PunchSprite, new Vector3(scale, scale, 1.0f));
    }

    private void HandleChargeSound()
    {
        if (chargeTime > minChargeTime)
        {
            if (falconSFXFlag == false)
            {
                audioManager.PlaySFX("ChargePunch1");
                audioManager.LoopSFX("ChargePunchBackground", true);
                audioManager.LoopSFX("ChargePunchBackground2", true);
                falconSFXFlag = true; falconSFXPlaying = true;
            }
        }
    }

    void HandleFinishSound()
    {
        if (falconSFXFlag == true)
        {
            additionalSFXWaitTime = punch1Length - sfxPlayTime;
            if (additionalSFXWaitTime > 0.00f) { Invoke("FinishSound", additionalSFXWaitTime); }
            else { Invoke("FinishSound", 0); }
        }
    }

    // Invoked in Handle Finish Sound, although reference says 0; this is due to Invoke allowing a wait time before executing
    void FinishSound() 
    {
        audioManager.LoopSFX("ChargePunchBackground", false);
        audioManager.LoopSFX("ChargePunchBackground2", false);
        audioManager.PlaySFX("ChargePunch2"); falconSFXFlag = false; sfxPlayTime = 0; falconSFXPlaying = false; 
    }

    private void HandleChargeVFX() 
    {
        if (chargeTime > minChargeTime) 
        {
            EventSystem.current.StartChargedAttackTrigger(holdTimeNormalized, gameObject.transform, null);
            if (particleTrigger == false) { ParticleSystemsOn(true); }
            playerVisualEffectsController.GlowOn(true);
        } 
    }

    void UpdateSpriteCorners()
    {
        (UpperRightCorner, BottomLeftCorner) = playerVisualEffectsController.GetSpriteUpperRightAndLowerLeftCorners(PunchSprite);
        playerVisualEffectsController.ToggleSpriteEnabled(PunchSprite, true); // enabled so that PlayerPrimaryWeapon can validate that it's on for collisions
    }

    void HandleFinishVFX()
    {
        if(chargeTime > minChargeTime) 
        { 
            StartCoroutine(playerVisualEffectsController.FadeInFadeOutSprite(PunchSprite, fadeDuration));
            StartCoroutine(CloseHitBoxAfterSpriteFade(fadeDuration));
        } // only call if hit a minimum threshold
        else { playerVisualEffectsController.ResetColorAndPosition(PunchSprite); }
    }

    IEnumerator CloseHitBoxAfterSpriteFade(float fadeDuration)
    {
        yield return new WaitForSeconds(fadeDuration);
        EventSystem.current.EndActiveMeleeTrigger();
    }

    void ParticleSystemsOn(bool status)
    {
        if (status == true)
        {
            // initial particles
            foreach(string particles in FirstWaveParticleSystems) 
            {
                playerVisualEffectsController.PlayParticleSystem(particles);
            }

            // first wave of effects after chargepunch start
            // specific to chargePunchElectricity and backing SFX track, stagger the start of electricity to match SFX backing track
            foreach (string particles in SecondWaveParticleSystems)
            {
                StartCoroutine(PlayParticleSystemWithDelay(particles, minChargeTime + firstWaveParticleSystemsTimeDelay));
            }
            TriggerOneTimeBuildFX();

            // second wave of effects
            foreach (string particles in ThirdWaveParticleSystems)
            {
                StartCoroutine(PlayParticleSystemWithDelay(particles, maxChargeTime));
            }
            particleTrigger = true;
        }
        else 
        { 
            foreach (string ps in AllParticleSystems) { playerVisualEffectsController.StopParticleSystem(ps); }
            particleTrigger = false;
        }
    }

    void TriggerOneTimeBuildFX() 
    {
        // make sure still charging before loading in, otherwise player may execute punch and these will fire after the fact
        if (IsCharging) 
        {
            cameraBehavior.ShakeScreen(0.5f);
            StartCoroutine(gameController.PlayHaptics());
        }
    }

    IEnumerator PlayParticleSystemWithDelay(string particleSystemName, float delay) 
    {
        yield return new WaitForSeconds(delay);
        if (IsCharging) { playerVisualEffectsController.PlayParticleSystem(particleSystemName); }
    }

    /// <summary>
    /// Called by specific key frame on player's base animator, so 
    /// </summary>
    void ReleasePunchOnFrame()
    {
        EventSystem.current.ActiveMeleeTrigger();
        if (playerAttackManager.MostRecentAttack.name == "Charge Punch") { playerAttackManager.MostRecentAttack.baseDamage = BaseDamage + DamageToPass; }
        ReleaseCharge();
    }
    
    /// <summary>
    /// Called if a player is hit while charging charge punch. 
    /// </summary>
    public void ReleaseChargeIfHit(Vector3 enemyPos, float knockbackMod, bool HitInActiveShieldZone)
    {
        ReleaseCharge();
    }

    public void ReleaseCharge() 
    {
        IsCharging = false;
        UpdateSpriteCorners();
        HandleFinishSound();
        HandleFinishVFX();
        ParticleSystemsOn(false);
        playerVisualEffectsController.GlowOn(false);
        chargeTime = 0;
        holdTimeNormalized = 0;
        EventSystem.current.FinishChargedAttackTrigger();
    }
}
