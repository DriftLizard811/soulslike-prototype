using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHP;
    public int currentHP;

    void Awake()
    {
        currentHP = maxHP;
    }
}
