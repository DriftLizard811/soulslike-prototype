using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTag : MonoBehaviour
{
    void Start()
    {
        GlobalData.global.enemyList.Add(this);
    }
}
