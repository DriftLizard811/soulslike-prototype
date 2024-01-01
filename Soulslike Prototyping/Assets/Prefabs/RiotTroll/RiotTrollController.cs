using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiotTrollController : MonoBehaviour
{
    Animator animator;
    [SerializeField] PlayerControllerV1 player;

    public enum riotTrollStates
    {
        idle,
        swing,
        backswing
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= 4f && Vector3.Distance(transform.position, player.transform.position) > 2.5f) {
            animator.SetBool("isSwing", true);
        }
        else if (Vector3.Distance(transform.position, player.transform.position) <= 2.5f) {
            animator.SetBool("isBash", true);
        }
    }

    public void ReturnToIdleState()
    {
        animator.SetBool("isSwing", false);
        animator.SetBool("isBackswing", false);
        animator.SetBool("isBash", false);
    }
}
