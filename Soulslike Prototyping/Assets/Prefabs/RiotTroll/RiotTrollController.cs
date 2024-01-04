using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RiotTrollController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    CharacterStats stats;
    [SerializeField] PlayerControllerV1 player;

    [SerializeField] List<AttackHitbox> attackHitboxes = new List<AttackHitbox>();

    public enum riotTrollStates
    {
        neutral = 0,
        swing = 1,
        backswing = 2,
        bash = 3
    }

    public riotTrollStates currentState = riotTrollStates.neutral;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<CharacterStats>();
        currentState = riotTrollStates.neutral;
    }

    void FixedUpdate()
    {
        CheckHealth();

        if (currentState == riotTrollStates.neutral) {
            //move the troll towards the player
            agent.SetDestination(player.transform.position);

            PickWalkAnimation();

            //see if an attack should be performed
            
            //see if the player is behind the enemy
            RaycastHit backHitInfo;
            var backRay = new Ray(transform.position - transform.up, (-transform.forward).normalized);
            var backHit = Physics.Raycast(backRay, out backHitInfo, 8f);
            if (backHit) {
                animator.SetBool("isBackswing", true);
                currentState = riotTrollStates.backswing;
            }

            //if not, proceed to normal attacks
            else if (Vector3.Distance(transform.position, player.transform.position) <= 3f && Vector3.Distance(transform.position, player.transform.position) > 2.5f) {
                //Debug.Log("swing activated");
                animator.SetBool("isSwing", true);
                currentState = riotTrollStates.swing;
            }
            else if (Vector3.Distance(transform.position, player.transform.position) <= 2.5f) {
                animator.SetBool("isBash", true);
                currentState = riotTrollStates.bash;
            }
        }
        else {
            //stop the enemy from moving
            agent.SetDestination(transform.position);
        }
    }

    void CheckHealth()
    {
        if (stats.currentHP <= 0) {
            Destroy(gameObject);
        }
    }

    void PickWalkAnimation()
    {
        if (agent.destination != transform.position) {
            animator.SetBool("isWalking", true);
        }
    }

    public void ActivateAttackHitbox()
    {
        //get the hitbox that matches the attack
        int hitboxToActivate = 0;
        hitboxToActivate = (int) currentState;

        //activate that hitbox
        attackHitboxes[hitboxToActivate].EnabledAttack();
    }

    public void DeactivateAttackHitbox()
    {
        //get the hitbox that matches the attack
        int hitboxToActivate = 0;
        hitboxToActivate = (int) currentState;

        //activate that hitbox
        attackHitboxes[hitboxToActivate].DisableAttack();
    }

    public void ReturnToIdleState()
    {
        currentState = riotTrollStates.neutral;
        animator.SetBool("isSwing", false);
        animator.SetBool("isBackswing", false);
        animator.SetBool("isBash", false);

        //turn off any attack hitboxes
        for (int i = 0; i < attackHitboxes.Count; i++) {
            if (attackHitboxes[i] != null) {
                attackHitboxes[i].DisableAttack();
            }
        }
    }
}
