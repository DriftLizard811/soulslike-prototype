using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class StatDisplay : MonoBehaviour
{
    OrbitingCamera orbitingCamera;

    [SerializeField] GameObject entity;
    CharacterStats entityStats;
    RectTransform rectTransform;

    //Setup vars
    [SerializeField] int displayHeight;
    [SerializeField] int pixelsPerUnit;

    public enum displayType
    {
        health,
        stamina,
        poise
    }

    [SerializeField] displayType type;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        entityStats = entity.GetComponent<CharacterStats>();

        DisplaySetup();
    }

    void LateUpdate()
    {
        if (entity != null) {
            if (orbitingCamera == null) {
                orbitingCamera = GlobalData.global.orbitingCamera;
            }
            if (orbitingCamera != null) {
                DisplayUpdate();   
            }
        }
    }

    void DisplaySetup()
    {
        if (type == displayType.health) {
            rectTransform.sizeDelta = new Vector2(pixelsPerUnit * entityStats.maxHP, displayHeight);
        }
        else if (type == displayType.stamina) {
            rectTransform.sizeDelta = new Vector2(pixelsPerUnit * entityStats.maxStamina, displayHeight);
        }
    }

    void DisplayUpdate()
    {
        if (type == displayType.health) {
            rectTransform.sizeDelta = new Vector2(pixelsPerUnit * entityStats.currentHP, displayHeight);
        }
        else if (type == displayType.stamina) {
            rectTransform.sizeDelta = new Vector2(pixelsPerUnit * Mathf.RoundToInt(entityStats.currentStamina), displayHeight);
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
