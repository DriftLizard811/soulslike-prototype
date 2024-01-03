using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalData : MonoBehaviour
{
    public static GlobalData global;

    [HideInInspector] public PlayerControllerV1 player;
    [HideInInspector] public OrbitingCamera orbitingCamera;
    public List<EnemyTag> enemyList = new List<EnemyTag>();

    //DEBUG SETTINGS
    public bool showHitboxes = false;

    void Awake()
    {
        global = this;
    }

    void FixedUpdate()
    {
        if (player.stats.currentHP <= 0) {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
