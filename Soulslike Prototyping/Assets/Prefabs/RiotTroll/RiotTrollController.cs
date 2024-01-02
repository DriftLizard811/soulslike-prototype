using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RiotTrollController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
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
        currentState = riotTrollStates.neutral;
    }

    void FixedUpdate()
    {
        if (currentState == riotTrollStates.neutral) {
            //move the troll towards the player
            agent.SetDestination(player.transform.position);

            PickWalkAnimation();

            //see if an attack should be performed
            if (Vector3.Distance(transform.position, player.transform.position) <= 3f && Vector3.Distance(transform.position, player.transform.position) > 2.5f) {
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

            //get the hitbox that matches the attack
            int hitboxToActivate = 0;
            hitboxToActivate = (int) currentState;

            //activate that hitbox
            attackHitboxes[hitboxToActivate].EnabledAttack();
        }
    }

    void PickWalkAnimation()
    {
        if (agent.destination != transform.position) {
            animator.SetBool("isWalking", true);
        }
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
