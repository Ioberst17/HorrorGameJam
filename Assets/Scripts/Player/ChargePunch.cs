using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerPrimaryWeapon))]
public class ChargePunch : MonoBehaviour
{
    PlayerPrimaryWeapon playerPrimaryWeapon;
    public float maxChargeTime = 2f;      // the maximum time the punch can be charged for
    public float chargeSpeed = 1f;        // the speed at which the punch charge increases
    public GameObject punchEffect;        // a particle effect that plays when the punch is released


    [SerializeField] public int damageToPass;
    [SerializeField] private float maxDamageToAdd = 10f;
    [SerializeField] private float holdTimeNormalized;
    [SerializeField] int attackDirection;

    [SerializeField] private float chargeTime;             // the current charge time
    [SerializeField] public bool isCharging;                // whether the punch is currently being charged

    private void Start()
    {
        playerPrimaryWeapon = GetComponent<PlayerPrimaryWeapon>();
    }

    public void Execute() { isCharging = true; }

    public void Release(int attackDirection) { this.attackDirection = attackDirection; isCharging = false; ReleasePunch(); }

    void Update()
    {
        if (isCharging)
        {
            chargeTime = chargeTime + (Time.deltaTime * chargeSpeed);
            CalcForce();
        }
    }

    private void CalcForce()
    {
        holdTimeNormalized = Mathf.Clamp01(chargeTime / maxChargeTime);
        damageToPass = (int)(holdTimeNormalized * maxDamageToAdd);
        if(chargeTime > 0.2f) { EventSystem.current.StartChargedAttackTrigger(holdTimeNormalized, gameObject.transform, null); }
    }

    void ReleasePunch()
    {
        playerPrimaryWeapon.damageToPass = playerPrimaryWeapon.minDamage + damageToPass;
        StartCoroutine(playerPrimaryWeapon.AttackActiveFrames(attackDirection));
        chargeTime = 0;
        isCharging = false;
        //Instantiate(punchEffect, transform.position, Quaternion.identity);
        EventSystem.current.FinishChargedAttackTrigger();
    }
}
