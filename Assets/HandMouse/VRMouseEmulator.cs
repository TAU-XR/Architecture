using UnityEngine;

public class VRMouseEmulator : MonoBehaviour
{
    public Transform fingerTransform; // The transform of the finger
    public GameObject targetObject; // The target object to move
    public Transform mousePlaneTransform; // The transform of the plane acting as the mouse pad

    public float maxDistanceXZ = 0.1f; // Maximum allowable distance in the x and z axes
    public float maxDistanceY = 0.01f; // Maximum allowable distance in the y axis
    public float movementMultiplier = 1.0f; // Multiplier to scale the movement of the target object
    public float lerpSpeed = 5.0f; // Speed of the lerp movement
    public float inertiaDuration = 0.5f; // Duration for which the object keeps moving after stopping

    private Vector3 initialFingerPositionLocal;
    private Vector3 initialTargetPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float inertiaTime = 0f;

    void Start()
    {
        if (targetObject == null || fingerTransform == null || mousePlaneTransform == null)
        {
            Debug.LogError("Please assign the finger transform, target object, and mouse plane in the inspector.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // Convert the finger's world position to the plane's local space
        Vector3 fingerPositionLocal = mousePlaneTransform.InverseTransformPoint(fingerTransform.position);

        // Calculate the distances along the y-axis and xz-plane
        float distanceY = Mathf.Abs(fingerPositionLocal.y);
        float distanceXZ = Mathf.Max(Mathf.Abs(fingerPositionLocal.x), Mathf.Abs(fingerPositionLocal.z));

        // Debugging logs
        Debug.Log($"Finger Local Position: {fingerPositionLocal}");
        Debug.Log($"Y Distance: {distanceY}, XZ Distance: {distanceXZ}");

        // Check if the finger is within the allowable distance
        bool withinDistance = distanceXZ <= maxDistanceXZ && distanceY <= maxDistanceY;

        Debug.Log($"Within Distance: {withinDistance}");

        if (withinDistance)
        {
            if (!isMoving)
            {
                // Store the initial positions when the finger enters the allowable distance
                initialFingerPositionLocal = fingerPositionLocal;
                initialTargetPosition = targetObject.transform.position;
                isMoving = true;
                inertiaTime = inertiaDuration; // Reset inertia timer
                Debug.Log("Started Moving");
            }

            // Calculate the movement delta of the finger in local space
            Vector3 fingerDeltaLocal = fingerPositionLocal - initialFingerPositionLocal;

            // Convert the delta back to world space, apply the multiplier, and restrict movement to x and z axes
            Vector3 fingerDeltaWorld = mousePlaneTransform.TransformDirection(fingerDeltaLocal) * movementMultiplier;
            fingerDeltaWorld.y = 0; // Ensure no movement on the y-axis

            targetPosition = initialTargetPosition + fingerDeltaWorld;
        }
        else if (isMoving)
        {
            inertiaTime -= Time.deltaTime;
            if (inertiaTime <= 0f)
            {
                isMoving = false;
                Debug.Log("Stopped Moving");
            }
        }

        // Smoothly move the target object towards the target position using Lerp
        targetObject.transform.position = Vector3.Lerp(targetObject.transform.position, targetPosition, Time.deltaTime * lerpSpeed);
        Debug.Log($"Target Position: {targetObject.transform.position}");
    }
}
