using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] string targetTag;
    [SerializeField] int damageAmount;
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
        if (GlobalData.global.showHitboxes) {
            mr.enabled = true;
        }
    }

    public void DisableAttack()
    {
        bc.enabled = false;
        if (GlobalData.global.showHitboxes) {
            mr.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == targetTag) {
            var otherStats = other.GetComponentInParent<CharacterStats>();

            if (otherStats != null) {
                otherStats.currentHP -= damageAmount;
            }
            else {
                Debug.LogError("Couldn't find CharacterStats component in collided object");
            }
        }
    }
}
