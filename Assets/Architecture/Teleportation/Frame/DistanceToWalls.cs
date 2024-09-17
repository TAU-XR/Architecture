using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction.Locomotion; // Import the teleport system namespace
using System.Collections;

public class DistanceToWallsWithLerpAndColor : MonoBehaviour
{
    public GameObject boxObject; // The box that the other object will be compared to
    public GameObject targetObject; // The object whose distance to the walls is calculated
    public GameObject frameObject; // The object whose shader properties will be lerped
    public float threshold = 0.5f; // The distance threshold
    public float minValue = 0f; // Min value for the shader's Float property
    public float maxValue = 1f; // Max value for the shader's Float property
    public Color targetColor = Color.white; // The target color to lerp towards
    public AnimationCurve lerpCurve = AnimationCurve.Linear(0, 0, 1, 1); // Lerp curve for the float
    public AnimationCurve colorLerpCurve = AnimationCurve.Linear(0, 0, 1, 1); // Lerp curve for the color
    public UnityEvent onThresholdReached; // Event to trigger when entering the threshold
    public UnityEvent onThresholdExited; // Event to trigger when exiting the threshold
    public PlayerLocomotor playerLocomotor; // Reference to the teleportation system script

    private Vector3 boxMin;
    private Vector3 boxMax;
    private bool isWithinThreshold = false; // Track if the object is within the threshold
    private Material frameMaterial; // The material of the frame object
    private Color originalColor; // The original color of the shader
    private int framesToSkip = 0; // Number of frames to skip lerp calculations

    void Start()
    {
        // Calculate the initial bounds of the box
        UpdateBoxBounds();

        // Get the material of the frame object
        Renderer frameRenderer = frameObject.GetComponent<Renderer>();
        if (frameRenderer != null)
        {
            frameMaterial = frameRenderer.material;
            // Store the original color of the material
            originalColor = frameMaterial.GetColor("_Color");
        }

        // Subscribe to the teleportation event
        if (playerLocomotor != null)
        {
            playerLocomotor.WhenLocomotionEventHandled += OnTeleport;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the teleportation event
        if (playerLocomotor != null)
        {
            playerLocomotor.WhenLocomotionEventHandled -= OnTeleport;
        }
    }

    private void OnTeleport(LocomotionEvent locomotionEvent, Pose delta)
    {
        // Stop the lerp calculation and distance checks for a few frames to allow the teleport to complete
        StartCoroutine(SkipLerpForFrames(10));

        // After teleport, update the box bounds
        UpdateBoxBounds();
    }

    private IEnumerator SkipLerpForFrames(int frameCount)
    {
        framesToSkip = frameCount;
        yield return null; // Return control to Unity for the next frame
    }

    private void UpdateBoxBounds()
    {
        // Update the BoxCollider or Renderer bounds of the box after teleportation
        BoxCollider boxCollider = boxObject.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxMin = boxCollider.bounds.min;
            boxMax = boxCollider.bounds.max;
        }
        else
        {
            Renderer boxRenderer = boxObject.GetComponent<Renderer>();
            if (boxRenderer != null)
            {
                boxMin = boxRenderer.bounds.min;
                boxMax = boxRenderer.bounds.max;
            }
        }
    }

    void Update()
    {
        // Skip distance calculation for the set number of frames after teleport
        if (framesToSkip > 0)
        {
            framesToSkip--;
            return; // Skip this frame
        }

        // After the skip period, we expect both the box and the target to have finished teleporting
        Vector3 targetPosition = targetObject.transform.position;

        // Calculate distances to each wall (min and max bounds in X and Z axes only)
        float distanceToLeft = Mathf.Abs(targetPosition.x - boxMin.x);
        float distanceToRight = Mathf.Abs(targetPosition.x - boxMax.x);
        float distanceToBack = Mathf.Abs(targetPosition.z - boxMin.z);
        float distanceToFront = Mathf.Abs(targetPosition.z - boxMax.z);

        // Find the smallest distance among the calculated distances
        float smallestDistance = Mathf.Min(distanceToLeft, distanceToRight, distanceToBack, distanceToFront);

        // Print the smallest distance to the console for debugging
        Debug.Log("Smallest distance from the box: " + smallestDistance);

        // Check if the object is within the threshold
        bool withinThreshold = smallestDistance < threshold;

        // Lerp the shader's float and color values if within the threshold
        if (withinThreshold)
        {
            // Calculate the normalized value (0 to 1) based on the distance
            float normalizedDistance = Mathf.Clamp01(1 - (smallestDistance / threshold));

            // Apply the lerp curve for the float value
            float curveValue = lerpCurve.Evaluate(normalizedDistance);

            // Lerp between the min and max values based on the curve
            float lerpValue = Mathf.Lerp(minValue, maxValue, curveValue);

            // Set the shader's "Float" property
            if (frameMaterial != null)
            {
                frameMaterial.SetFloat("_Float", lerpValue);
            }

            // Apply the lerp curve for the color value
            float colorCurveValue = colorLerpCurve.Evaluate(normalizedDistance);

            // Lerp between the original color and target color based on the curve
            Color lerpedColor = Color.Lerp(originalColor, targetColor, colorCurveValue);

            // Set the shader's "Color" property
            if (frameMaterial != null)
            {
                frameMaterial.SetColor("_Color", lerpedColor);
            }
        }

        // Trigger the appropriate events
        if (withinThreshold && !isWithinThreshold)
        {
            onThresholdReached.Invoke();
        }
        else if (!withinThreshold && isWithinThreshold)
        {
            onThresholdExited.Invoke();
        }

        // Update the threshold tracking state
        isWithinThreshold = withinThreshold;
    }
}
