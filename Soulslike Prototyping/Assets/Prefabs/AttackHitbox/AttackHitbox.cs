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
        if (other.tag == targetTag && other.GetType() == typeof(CharacterController)) {
            var otherStats = other.GetComponentInParent<CharacterStats>();

            if (otherStats != null) {
                if (!otherStats.isInvincible) {
                    float defenseFactor = (100 - otherStats.defense) / 100;
                    int damageToDeal = Mathf.CeilToInt(damageAmount * defenseFactor);
                    otherStats.currentHP -= damageToDeal;
                    otherStats.poiseMeter += damageToDeal;
                    //Debug.LogFormat("(100 - {0}) / 100 = {1}", otherStats.defense, defenseFactor);

                    if (other.tag == "Player" && targetTag == "Player") {
                        var playerScript = other.GetComponentInParent<PlayerControllerV1>();
                        playerScript.ReturnToIdleState();
                        playerScript.TakeDamage((other.transform.position - transform.position).normalized);
                    }
                }
            }
            else {
                Debug.LogError("Couldn't find CharacterStats component in collided object");
            }
        }
    }
}
