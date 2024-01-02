using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] string targetTag;
    [SerializeField] Color displayColor;
    BoxCollider bc;
    MeshRenderer mr;

    void Awake()
    {
        bc = GetComponent<BoxCollider>();
        mr = GetComponent<MeshRenderer>();
        bc.enabled = false;
        mr.enabled = false;
    }

    public void EnabledAttack()
    {
        bc.enabled = true;
        mr.enabled = true;
    }

    public void DisableAttack()
    {
        bc.enabled = false;
        mr.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == targetTag) {
            Debug.Log("HIT");
        }
    }
}
