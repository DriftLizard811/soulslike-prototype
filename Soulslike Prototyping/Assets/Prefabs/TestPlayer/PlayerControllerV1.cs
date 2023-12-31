using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerV1 : MonoBehaviour
{
    CharacterController controller;
    [SerializeField] OrbitingCamera orbitingCamera;

    //player movement
    Vector3 inputDirection;
    [SerializeField] float moveSpeed = 3f;
    [HideInInspector] public Quaternion cameraRotation;
    [HideInInspector] public Transform cameraTransform;

    Vector3 gravityVector = Vector3.zero;
    [SerializeField] float gravityFactor = 9.8f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        gravityVector.y -= gravityFactor * Time.fixedDeltaTime;
        controller.Move(gravityVector);

        if (inputDirection != Vector3.zero) {
            //create the global move that the player will use
            Vector3 moveDirection = inputDirection.x * cameraTransform.right + inputDirection.z * cameraTransform.forward;
            
            //move the player
            controller.Move(moveDirection * moveSpeed * Time.fixedDeltaTime);

            //rotate so that the player's back is to the camera
            transform.rotation = orbitingCamera.playerRotation;
        }
    }

    void OnMove(InputValue movementValue)
    {
        inputDirection = new Vector3(movementValue.Get<Vector2>().x, 0, movementValue.Get<Vector2>().y);
    }
}
