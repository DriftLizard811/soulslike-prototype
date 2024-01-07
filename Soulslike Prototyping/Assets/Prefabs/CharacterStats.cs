using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHP;
    public int currentHP;
    public bool isInvincible = false;

    public float defense;

    public float maxStamina;
    public float currentStamina;
    float staminaPerFrame = 0.2f;

    public float poiseCap;
    public float poiseMeter;
    float poiseRechargePerFrame = 0.1f;
    public bool isStaggered;

    void Awake()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
    }

    void FixedUpdate()
    {
        if (currentStamina < maxStamina) {
            currentStamina += staminaPerFrame;
        }
        if (poiseMeter > poiseCap) {
            isStaggered = true;
        }
        if (poiseMeter > 0) {
            poiseMeter -= poiseRechargePerFrame;
        }
    }
}
