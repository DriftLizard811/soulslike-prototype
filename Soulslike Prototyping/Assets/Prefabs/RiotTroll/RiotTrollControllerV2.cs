using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RiotTrollControllerV2 : MonoBehaviour
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

            //get distance to player
            Vector3 vectorToPlayer = (player.transform.position - transform.position);
            float distanceToPlayer = vectorToPlayer.magnitude;
            Debug.Log(distanceToPlayer);

            //get the angle to the player
            vectorToPlayer.y = 0;
            vectorToPlayer.Normalize();
            float angleToPlayer = Vector3.Angle(transform.forward, vectorToPlayer);

            if (angleToPlayer <= 20 && distanceToPlayer < 4f) {
                animator.SetBool("isBash", true);
                currentState = riotTrollStates.bash;
            }
            else if (angleToPlayer > 20 && angleToPlayer <= 40 && distanceToPlayer < 4f) {
                animator.SetBool("isSwing", true);
                currentState = riotTrollStates.swing;
            }
            else if (angleToPlayer > 160 && distanceToPlayer < 4f) {
                animator.SetBool("isBackswing", true);
                currentState = riotTrollStates.backswing;
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
