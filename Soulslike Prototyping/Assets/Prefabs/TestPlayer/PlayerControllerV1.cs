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
    [SerializeField] float sidestepSpeed = 6f;
    [HideInInspector] public Quaternion cameraRotation;
    public Transform cameraTransform;

    //sidestep variables
    Vector3 sidestepDirection = Vector3.zero;

    //gravity tracking variables
    public Vector3 gravityVector = Vector3.zero;
    [SerializeField] float gravityFactor = 9.8f;

    //keep track of player state
    public enum playerStates
    {
        neutral,
        swing,
        sidestep,
        roll,
        damaged
    }
    playerStates currentState = playerStates.neutral;

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

                //move the player
                controller.Move(moveDirection.normalized * moveSpeed * Time.fixedDeltaTime);

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
        if (currentState == playerStates.sidestep) {
            if (orbitingCamera.isLockedOn) {
                Vector3 moveDirection = inputDirection.x * cameraTransform.right + inputDirection.z * cameraTransform.forward;
                moveDirection.y = 0;

                controller.Move(moveDirection.normalized * sidestepSpeed * Time.fixedDeltaTime);

                //rotate player to keep back to camera
                transform.rotation = orbitingCamera.playerRotation;
            }
            else {
                controller.Move(sidestepDirection * sidestepSpeed * Time.fixedDeltaTime);
            }
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

        //turn off hitboxes

        //re-enable camera moving
        orbitingCamera.canRotateCamera = true;

        //return state to neutral
        currentState = playerStates.neutral;
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
        if (inputDirection != Vector3.zero) {
            //play animation
            animator.SetBool("isSidestep", true);

            //get sidestep direction
            sidestepDirection = inputDirection.x * cameraTransform.right + inputDirection.z * cameraTransform.forward;
            sidestepDirection.y = 0;
            sidestepDirection = sidestepDirection.normalized;

            //enter sidestep state
            currentState = playerStates.sidestep;

            //disable rotating camera ability
            orbitingCamera.canRotateCamera = false;
        }
    }
}
