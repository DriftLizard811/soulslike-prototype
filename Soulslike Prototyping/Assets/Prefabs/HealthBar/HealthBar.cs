using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore;

public class HealthBar : MonoBehaviour
{
    OrbitingCamera orbitingCamera;

    [SerializeField] GameObject entity;
    CharacterStats entityStats;

    [SerializeField] RectTransform uiTransform;

    TextMeshProUGUI textBox;
    RectTransform rectTransform;

    public enum displayType
    {
        health,
        stamina,
        poise
    }

    [SerializeField] displayType type;

    void Awake()
    {
        textBox = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        entityStats = entity.GetComponent<CharacterStats>();
    }

    void LateUpdate()
    {
        if (entity != null) {
            if (orbitingCamera == null) {
                orbitingCamera = GlobalData.global.orbitingCamera;
            }
            if (orbitingCamera != null) {
                if (type == displayType.health) {
                    textBox.text = string.Format("{0}: {1}/{2}", entity.name, Mathf.RoundToInt(entityStats.currentHP), entityStats.maxHP);
                }
                else if (type == displayType.stamina) {
                    textBox.text = string.Format("{0}: {1}/{2}", entity.name, Mathf.RoundToInt(entityStats.currentStamina), entityStats.maxStamina);
                }
                else if (type == displayType.poise) {
                    textBox.text = string.Format("{0}: {1}/{2}", entity.name, Mathf.RoundToInt(entityStats.poiseMeter), entityStats.poiseCap);
                }
            }
        }
    }

    bool IsPointOnScreen(Vector3 point)
    {
        var screenPoint = Camera.main.WorldToScreenPoint(point);
        //Debug.Log(screenPoint);
        if (screenPoint.x >= 0 && screenPoint.x <= orbitingCamera.screenWidth && screenPoint.y >= 0 && screenPoint.y <= orbitingCamera.screenHeight && screenPoint.z >= 0) {
            return true;
        }
        else {
            return false;
        }
    }
}
