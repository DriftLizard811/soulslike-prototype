using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
    public static GlobalData global;

    [HideInInspector] public PlayerControllerV1 player;
    [HideInInspector] public OrbitingCamera orbitingCamera;

    void Awake()
    {
        global = this;
    }
}
