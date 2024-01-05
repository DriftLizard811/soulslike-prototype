using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerV1 : MonoBehaviour
{
    CharacterController controller;
    Animator animator;
    public OrbitingCamera orbitingCamera;

    //get stats component
    [HideInInspector] public CharacterStats stats;

    //player movement
    Vector3 inputDirection;
    [HideInInspector] public Vector2 lookDirection;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float enemyCollisionOffset = 0.5f;
    [SerializeField] float sidestepSpeed = 6f;
    [HideInInspector] public Quaternion cameraRotation;
    public Transform cameraTransform;

    //sidestep variables
    Vector3 dodgeDirection = Vector3.zero;

    //damaged variable
    [SerializeField] float damagedSpeed = 6f;
    Vector3 damagedDirection = Vector3.zero;

    //gravity tracking variables
    public Vector3 gravityVector = Vector3.zero;
    [SerializeField] float gravityFactor = 9.8f;

    //keep track of player state
    public enum playerStates
    {
        neutral,
        stab,
        sidestep,
        roll,
        damaged
    }
    [HideInInspector] public playerStates currentState = playerStates.neutral;

    [SerializeField] List<AttackHitbox> attackHitboxes = new List<AttackHitbox>();

    void Awake()
    {
        //get components
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        stats = GetComponent<CharacterStats>();
    }

    void Start()
    {
        //put self-reference in the globalData player variable
        GlobalData.global.player = this;
        
        orbitingCamera = GlobalData.global.orbitingCamera;
    }

    void LateUpdate()
    {
        if (orbitingCamera == null) {
            orbitingCamera = GlobalData.global.orbitingCamera;
            cameraTransform = orbitingCamera.transform;
        }
        else if (cameraTransform == null) {
            cameraTransform = orbitingCamera.transform;
        }
    }

    void FixedUpdate()
    {
        if (currentState == playerStates.neutral) {
            UpdateGravity();

            if (inputDirection != Vector3.zero && orbitingCamera != null) {
                //create the global move that the player will use
                Vector3 moveDirection = inputDirection.x * cameraTransform.right + inputDirection.z * cameraTransform.forward;
                moveDirection.y = 0;
                moveDirection.Normalize();

                //move the player
                bool success = TryMove(moveDirection, moveSpeed * Time.fixedDeltaTime + enemyCollisionOffset, moveSpeed);
                if (!success) {
                    success = TryMove(new Vector3(moveDirection.x, 0, 0), moveSpeed * Time.fixedDeltaTime + enemyCollisionOffset, moveSpeed);

                    if (!success) {
                        TryMove(new Vector3(0, 0, moveDirection.z), moveSpeed * Time.fixedDeltaTime + enemyCollisionOffset, moveSpeed);
                    }
                }
                

                //animate walk
                animator.SetBool("isWalking", true);

                //rotate so that the player's back is to the camera
                transform.rotation = orbitingCamera.playerRotation;
            }
            else {
                //return to idle
                animator.SetBool("isWalking", false);
            }
        }
        else if (currentState == playerStates.sidestep) {
            Vector3 moveDirection = inputDirection.x * cameraTransform.right + inputDirection.z * cameraTransform.forward;
            moveDirection.y = 0;
            moveDirection.Normalize();

            TryMove(moveDirection, sidestepSpeed * Time.fixedDeltaTime + enemyCollisionOffset, sidestepSpeed);

            //rotate player to keep back to camera
            transform.rotation = orbitingCamera.playerRotation;
        }
        else if (currentState == playerStates.roll) {
            TryMove(dodgeDirection, sidestepSpeed * Time.fixedDeltaTime + enemyCollisionOffset, sidestepSpeed);
        }
        else if (currentState == playerStates.stab) {

        }
        else if (currentState == playerStates.damaged) {
            TryMove(damagedDirection, damagedSpeed * Time.fixedDeltaTime + enemyCollisionOffset, damagedSpeed);
        }
    }

    //checks to make sure that the player isn't running into an enemy
    bool TryMove(Vector3 direction, float distance, float speed)
    {
        //Debug.Log(distance);
        RaycastHit hitInfo;
        var moveRay = new Ray(transform.position, direction);
        var moveHit = Physics.Raycast(moveRay, out hitInfo, distance);

        Debug.Log(hitInfo.distance);
        if (hitInfo.collider != null) {
            if (hitInfo.collider.tag == "Enemy") {
                return false;
            }
            else {
                controller.Move(direction * speed * Time.fixedDeltaTime);
                return true;
            }
        }
        else {
            controller.Move(direction * speed * Time.fixedDeltaTime);
            return true;
        }
    }

    //update the gravity acting on the player
    void UpdateGravity() {
        //if the player is not grounded, then apply gravity
        if (!controller.isGrounded) {
            gravityVector.y -= gravityFactor * Time.fixedDeltaTime * Time.fixedDeltaTime;
            controller.Move(gravityVector);
        }

        //else reset gravity to 0
        else {
            gravityVector.y = 0;
        }
    }

    //enable/disable attack hitboxes
    public void EnableAttack()
    {
        int hitboxIndex = (int) currentState;
        Debug.Log(hitboxIndex);

        if (attackHitboxes[hitboxIndex] != null) {
            attackHitboxes[hitboxIndex].EnabledAttack();
        }
    }
    public void DisableAttack()
    {
        int hitboxIndex = (int) currentState;

        if (attackHitboxes[hitboxIndex] != null) {
            attackHitboxes[hitboxIndex].DisableAttack();
        }
    }

    //initiate taking damage
    public void TakeDamage(Vector3 direction) {
        //play animation
        animator.SetBool("isDamaged", true);

        //set damaged direction
        damagedDirection = direction;

        //lock camera
        orbitingCamera.canRotateCamera = false;

        //change state
        currentState = playerStates.damaged;
    }

    //determines if a screen point is on screen
    bool IsPointOnScreen(Vector3 point)
    {
        var screenPoint = Camera.main.WorldToScreenPoint(point);
        Debug.Log(screenPoint);
        if (screenPoint.x >= 0 && screenPoint.x <= orbitingCamera.screenWidth && screenPoint.y >= 0 && screenPoint.y <= orbitingCamera.screenHeight && screenPoint.z >= 0) {
            return true;
        }
        else {
            return false;
        }
    }

    //return to the player's idle/neutral state after an action
    public void ReturnToIdleState()
    {
        //turn off any actions
        animator.SetBool("isSidestep", false);
        animator.SetBool("isRoll", false);
        animator.SetBool("isStab", false);
        animator.SetBool("isDamaged", false);

        //turn off any attack hitboxes
        for (int i = 0; i < attackHitboxes.Count; i++) {
            if (attackHitboxes[i] != null) {
                attackHitboxes[i].DisableAttack();
            }
        }

        //re-enable camera moving
        orbitingCamera.canRotateCamera = true;

        //return state to neutral
        currentState = playerStates.neutral;
    }

    //toggle invincibility
    public void EnableInvincibility()
    {
        stats.isInvincible = true;
    }

    public void DisableInvincibility()
    {
        stats.isInvincible = false;
    }

    void OnMove(InputValue movementValue)
    {
        inputDirection = new Vector3(movementValue.Get<Vector2>().x, 0, movementValue.Get<Vector2>().y);
    }

    void OnLook(InputValue lookValue)
    {
        lookDirection = lookValue.Get<Vector2>();   
    }

    void OnLock()
    {
        if (currentState == playerStates.neutral) {
            if (orbitingCamera.isLockedOn) {
                //turn off lock on
                orbitingCamera.isLockedOn = false;
            }
            else {
                if (GlobalData.global.enemyList.Count > 0) {
                    List<EnemyTag> globalEnemyList = GlobalData.global.enemyList;
                    Transform target = null;

                    //pick which enemy to lock on to
                    for (int i = 0; i < globalEnemyList.Count; i++) {
                        if (IsPointOnScreen(globalEnemyList[i].transform.position)) {
                            if (target == null) {
                                target = globalEnemyList[i].transform;
                            }
                            else {
                                float targetDistance = Vector2.Distance(Camera.main.WorldToScreenPoint(target.position), orbitingCamera.screenCenter);
                                float testDistance = Vector2.Distance(Camera.main.WorldToScreenPoint(globalEnemyList[i].transform.position), orbitingCamera.screenCenter);

                                if (testDistance < targetDistance) {
                                    target = globalEnemyList[i].transform;
                                }
                            }
                        }
                    }

                    if (target == null) {

                    }
                    else {
                        orbitingCamera.lockOnTarget = target;

                        orbitingCamera.isLockedOn = true;
                    }
                }
            }
        }
    }

    void OnDodge()
    {
        if (currentState == playerStates.neutral && stats.currentStamina >= 15) {
            //decrease stamina
            stats.currentStamina -= 15;

            if (inputDirection != Vector3.zero && orbitingCamera.isLockedOn) {
                //play animation
                animator.SetBool("isSidestep", true);

                //enter sidestep state
                currentState = playerStates.sidestep;
            }
            else if (inputDirection != Vector3.zero && !orbitingCamera.isLockedOn) {
                //play animation
                animator.SetBool("isRoll", true);

                //get dodge direction
                dodgeDirection = inputDirection.x * cameraTransform.right + inputDirection.z * cameraTransform.forward;
                dodgeDirection.y = 0;
                dodgeDirection = dodgeDirection.normalized;

                //disable rotating camera ability
                orbitingCamera.canRotateCamera = false;

                //enter roll state
                currentState = playerStates.roll;
            }
        }
    }

    void OnPrimary()
    {
        if (currentState == playerStates.neutral && stats.currentStamina >= 10) {
            //decrease stamina
            stats.currentStamina -= 10;

            //play animation
            animator.SetBool("isStab", true);

            //disable camera rotation
            orbitingCamera.canRotateCamera = false;

            //enter stab state
            currentState = playerStates.stab;
        }

    }
}
