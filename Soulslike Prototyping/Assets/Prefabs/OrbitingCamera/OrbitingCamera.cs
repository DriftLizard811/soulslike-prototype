using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingCamera : MonoBehaviour
{
    //object being rotated around
    public PlayerControllerV1 target;
    [HideInInspector] public Quaternion playerRotation;

    //the speeds at which rotation and elevation are changed
    [SerializeField] float rotationSpeed = 120.0f;
    [SerializeField] float elevationSpeed = 120.0f;

    //minimum and maximum angle of elevation
    [SerializeField] float elevationMinLimit = -20f;
    [SerializeField] float elevationMaxLimit = 80f;

    //distance we're at from the target
    [SerializeField] float distance = 5.0f;
    [SerializeField] float distanceMin = 0.5f;
    [SerializeField] float distanceMax = 15f;

    //angle at which we're rotated around the target
    float rotationAroundTarget = 0.0f;

    //the angle at which we're looking down or up at the target
    float elevationToTarget = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationAroundTarget = angles.y;
        elevationToTarget = angles.x;

        if (target) {
            float currentDistance = (transform.position - target.transform.position).magnitude;

            target.cameraTransform = transform;

            //distance = Mathf.Clamp(currentDistance, distanceMin, distanceMax);
        }
    }

    void LateUpdate()
    {
        if (target) {
            rotationAroundTarget += Input.GetAxis("Mouse X") * rotationSpeed * distance * 0.02f;
            elevationToTarget -= Input.GetAxis("Mouse Y") * elevationSpeed * 0.02f;

            //limit elevation to the range
            elevationToTarget = ClampAngle(elevationToTarget, elevationMinLimit, elevationMaxLimit);

            //compute rotation based on these two angles, and compute a player rotation as well
            Quaternion rotation = Quaternion.Euler(elevationToTarget, rotationAroundTarget, 0);
            playerRotation = Quaternion.Euler(-90, rotationAroundTarget, 0);

            //figure out a position that's distance units away from the target in the reverse direction to what we're looking in
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.transform.position;

            //update position and rotation
            transform.position = position;
            transform.rotation = rotation;

            //update the player rotation
            target.cameraRotation = playerRotation;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        //wrap the angle at -360 and 360
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;

        //clamp wrapped angle
        return Mathf.Clamp(angle, min, max);
    }
}
