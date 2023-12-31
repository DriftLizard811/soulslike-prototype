using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerV1 : MonoBehaviour
{
    CharacterController controller;
    OrbitingCamera orbitingCamera;

    //player movement
    Vector3 inputDirection;
    [HideInInspector] public Vector2 lookDirection;
    [SerializeField] float moveSpeed = 3f;
    [HideInInspector] public Quaternion cameraRotation;
    [HideInInspector] public Transform cameraTransform;

    public Vector3 gravityVector = Vector3.zero;
    [SerializeField] float gravityFactor = 9.8f;

    void Awake()
    {
        //get components
        controller = GetComponent<CharacterController>();

        //put self-reference in the globalData player variable
        GlobalData.global.player = this;
    }

    void Start()
    {
        orbitingCamera = GlobalData.global.orbitingCamera;
    }

    void FixedUpdate()
    {
        UpdateGravity();

        if (inputDirection != Vector3.zero) {
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
            orbitingCamera.isLockedOn = true;
        }
    }
}
