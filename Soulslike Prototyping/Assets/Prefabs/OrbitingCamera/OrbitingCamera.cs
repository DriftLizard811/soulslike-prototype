using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class OrbitingCamera : MonoBehaviour
{
    //object being rotated around
    PlayerControllerV1 target;
    [HideInInspector] public Quaternion playerRotation;

    [HideInInspector] public float screenWidth;
    [HideInInspector] public float screenHeight;
    [HideInInspector] public Vector2 screenCenter = Vector2.zero;

    [SerializeField] bool clipCamera;
    public bool isLockedOn = false;
    public Transform lockOnTarget;

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

    void Awake()
    {

        //get the screen width and height
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        screenCenter = new Vector2(screenWidth / 2, screenHeight / 2);
    }

    void Start()
    {
        GlobalData.global.orbitingCamera = this;

        //configure the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //fetch player reference from global data
        target = GlobalData.global.player;

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
            //set up position and rotation variables
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            //if the player is locked on to a target, configure the camera so that it's pointing at the enemy, while still keeping the player in view
            if (isLockedOn) {
                //get the angles between the player and the lock on target
                Vector3 rotationAngles = FindLockOnTargetRotation(target.transform, lockOnTarget);

                //Debug.Log(rotationAngles);

                //compute rotation based on the rotation angles
                rotation = Quaternion.Euler(rotationAngles.x, rotationAngles.y, 0);
                playerRotation = Quaternion.Euler (-90, rotationAngles.y, 0);

                //figure out a position that's distance units away from the target in the reverse direction to what we're looking in
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                position = rotation * negDistance + target.transform.position;
            }

            //if the player is not locked on to a target, adjust camera angle based on player input
            if (!isLockedOn) {
                Vector2 playerLookDirection = GetLookDirection(target);

                rotationAroundTarget += playerLookDirection.x * rotationSpeed * distance * 0.02f;
                elevationToTarget -= playerLookDirection.y * elevationSpeed * 0.02f;

                //limit elevation to the range
                elevationToTarget = ClampAngle(elevationToTarget, elevationMinLimit, elevationMaxLimit);

                //compute rotation based on these two angles, and compute a player rotation as well
                rotation = Quaternion.Euler(elevationToTarget, rotationAroundTarget, 0);
                playerRotation = Quaternion.Euler(-90, rotationAroundTarget, 0);

                //figure out a position that's distance units away from the target in the reverse direction to what we're looking in
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                position = rotation * negDistance + target.transform.position;
            }

            //make sure that the camera doesn't clip through walls
            if (clipCamera) {

                //store data about any potential hit
                RaycastHit hitInfo;

                //create the ray to be cast from the target towards the camera
                var ray = new Ray(target.transform.position, position - target.transform.position);

                //store a potential hit, true means hit
                var hit = Physics.Raycast(ray, out hitInfo, distance);

                //if there was a hit, adjust the position to reflect that
                if (hit) {
                    position = hitInfo.point;
                }
            }

            //update position and rotation
            transform.position = position;
            transform.rotation = rotation;

            //update the player rotation
            target.cameraRotation = playerRotation;
        }
        else {
            target = target = GlobalData.global.player;
        }
    }

    Vector2 GetLookDirection(PlayerControllerV1 target)
    {
        if (target.lookDirection != Vector2.zero) {
            return target.lookDirection;
        }
        else {
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
    }

    Vector3 FindLockOnTargetRotation(Transform origin, Transform target)
    {
        //create variables to store the x and y rotations
        float xRotationValue = 0f;
        float yRotationValue = 0f;

        //find the x, y, and z components of the vector between origin and target
        float xComp = Mathf.Abs(origin.position.x - target.position.x);
        float yComp = Mathf.Abs(origin.position.y - target.position.y);
        float zComp = Mathf.Abs(origin.position.z - target.position.z);

        //find the angles to store in the rotation values
        yRotationValue = Mathf.Atan(zComp / xComp);
        xRotationValue = Mathf.Atan(yComp / (Mathf.Sqrt(xComp * xComp + zComp * zComp)));

        //convert the angles from radians to degrees
        xRotationValue *= Mathf.Rad2Deg;
        yRotationValue *= Mathf.Rad2Deg;

        //adjust angles for proper signs
        if (origin.position.x <= target.position.x && origin.position.z <= target.position.z) {
            //Debug.Log("upperright");
            yRotationValue -= (-90 + yRotationValue * 2);
        }
        else if (origin.position.x > target.position.x && origin.position.z <= target.position.z) {
            //Debug.Log("upperleft");
            yRotationValue -= 90;
        }
        else if (origin.position.x > target.position.x && origin.position.z > target.position.z) {
            //Debug.Log("lowerleft");
            yRotationValue -= (90 + yRotationValue * 2);
        }
        else if (origin.position.x <= target.position.x && origin.position.z > target.position.z) {
            //Debug.Log("lowerright");
            yRotationValue -= 270;
        }
        

        //return the quaternion to properly rotate
        return new Vector3(xRotationValue, yRotationValue, 0);
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
