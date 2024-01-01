using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerV1 : MonoBehaviour
{
    CharacterController controller;
    public OrbitingCamera orbitingCamera;

    //player movement
    Vector3 inputDirection;
    [HideInInspector] public Vector2 lookDirection;
    [SerializeField] float moveSpeed = 3f;
    [HideInInspector] public Quaternion cameraRotation;
    public Transform cameraTransform;

    public Vector3 gravityVector = Vector3.zero;
    [SerializeField] float gravityFactor = 9.8f;

    void Awake()
    {
        //get components
        controller = GetComponent<CharacterController>();

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
        UpdateGravity();

        if (inputDirection != Vector3.zero && orbitingCamera != null) {
            //create the global move that the player will use
            Vector3 moveDirection = inputDirection.x * cameraTransform.right + inputDirection.z * cameraTransform.forward;
            
            //move the player
            controller.Move(moveDirection * moveSpeed * Time.fixedDeltaTime);

            //rotate so that the player's back is to the camera
            transform.rotation = orbitingCamera.playerRotation;
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
        if (screenPoint.x >= 0 && screenPoint.x <= orbitingCamera.screenWidth && screenPoint.y >= 0 && screenPoint.y <= orbitingCamera.screenHeight) {
            return true;
        }
        else {
            return false;
        }
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
        if (orbitingCamera.isLockedOn) {
            orbitingCamera.isLockedOn = false;
        }
        else {
            if (GlobalData.global.enemyList.Count > 0) {
                List<EnemyTag> globalEnemyList = GlobalData.global.enemyList;
                Transform target = null;

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
