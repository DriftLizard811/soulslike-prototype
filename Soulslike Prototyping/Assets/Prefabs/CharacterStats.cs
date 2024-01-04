using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHP;
    public int currentHP;
    public bool isInvincible = false;

    public float maxStamina;
    public float currentStamina;
    float staminaPerFrame = 0.1f;

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
    }
}
