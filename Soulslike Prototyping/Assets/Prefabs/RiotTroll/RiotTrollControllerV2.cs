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

    [SerializeField] float walkSpeed = 2.5f;
    [SerializeField] float runSpeed = 5;

    float timeSinceAttack = 0f;

    [SerializeField] List<AttackHitbox> attackHitboxes = new List<AttackHitbox>();

    public enum riotTrollStates
    {
        neutral = 0,
        swing = 1,
        backswing = 2,
        bash = 3,
        smash = 4,
        stagger = 5
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
        CheckStagger();

        if (currentState == riotTrollStates.neutral) {
            //move the troll towards the player
            agent.SetDestination(player.transform.position);

            PickWalkAnimation();

            //get distance to player
            Vector3 vectorToPlayer = (player.transform.position - transform.position);
            float distanceToPlayer = vectorToPlayer.magnitude;
            //Debug.Log(distanceToPlayer);

            //get the angle to the player
            vectorToPlayer.y = 0;
            vectorToPlayer.Normalize();
            float angleToPlayer = Vector3.Angle(transform.forward, vectorToPlayer);

            if (angleToPlayer > 20 && angleToPlayer <= 50 && distanceToPlayer < 5f && stats.currentStamina >= 20 && timeSinceAttack <= Time.fixedDeltaTime * 3) {
                Debug.Log("follow-up");
                stats.currentStamina -= 15;

                animator.SetBool("isSwing", true);
                currentState = riotTrollStates.swing;
            }
            if (angleToPlayer <= 20 && distanceToPlayer < 4f && stats.currentStamina >= 15) {
                stats.currentStamina -= 15;

                animator.SetBool("isBash", true);
                currentState = riotTrollStates.bash;
            }
            else if (angleToPlayer > 20 && angleToPlayer <= 40 && distanceToPlayer < 3.5f && stats.currentStamina >= 10) {
                stats.currentStamina -= 10;

                animator.SetBool("isSwing", true);
                currentState = riotTrollStates.swing;
            }
            else if (angleToPlayer > 110 && distanceToPlayer < 4f && stats.currentStamina >= 20) {
                stats.currentStamina -= 20;

                animator.SetBool("isBackswing", true);
                currentState = riotTrollStates.backswing;
            }
            else if (distanceToPlayer < 4.5f && stats.currentStamina >= 40) {
                stats.currentStamina -= 30;

                animator.SetBool("isSmash", true);
                currentState = riotTrollStates.smash;
            }
            else {
                timeSinceAttack += Time.fixedDeltaTime;
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
            if (Vector3.Distance(transform.position, player.transform.position) > 6f) {
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);
                agent.speed = runSpeed;
            }
            else {
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                agent.speed = walkSpeed;
            }
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

    void CheckStagger()
    {
        if (stats.poiseMeter >= stats.poiseCap) {
            animator.SetBool("isStagger", true);
            //ResetAnimatorBools();
            currentState = riotTrollStates.stagger;
        }
    }

    public void ResetPoise()
    {
        stats.poiseMeter = 0;
        stats.isStaggered = false;
    }

    public void ResetAnimatorBools()
    {
        animator.SetBool("isSwing", false);
        animator.SetBool("isBackswing", false);
        animator.SetBool("isBash", false);
        animator.SetBool("isSmash", false);

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
    }

    public void ReturnToIdleState()
    {
        currentState = riotTrollStates.neutral;
        ResetAnimatorBools();
        animator.SetBool("isStagger", false);

        timeSinceAttack = 0f;

        //turn off any attack hitboxes
        for (int i = 0; i < attackHitboxes.Count; i++) {
            if (attackHitboxes[i] != null) {
                attackHitboxes[i].DisableAttack();
            }
        }
    }
}
