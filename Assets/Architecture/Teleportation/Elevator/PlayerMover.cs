using System.Collections; // Required for IEnumerator
using UnityEngine;

// Import Meta's Locomotion namespaces
using Oculus.Interaction.Locomotion;

public class PlayerMover : MonoBehaviour
{
    // -------------------------------------------------------------------
    // PUBLIC FIELDS (assign in Inspector)
    // -------------------------------------------------------------------

    [Header("References")]
    [Tooltip("The main Player GameObject or OVRCameraRig root.")]
    public GameObject player;

    [Tooltip("SkinnedMeshRenderer with the blend shape (e.g., a door).")]
    public SkinnedMeshRenderer blendShapeObject;

    [Tooltip("Separate object that moves along with the player (e.g., door transform).")]
    public GameObject blendShapeTransformObject;

    [Header("Blend Shape Settings")]
    [Tooltip("Index of the blend shape you want to animate (default 'Key 1').")]
    public int blendShapeIndex = 0;

    [Tooltip("Duration for lerping the blend shape from 1?0 and 0?1.")]
    public float lerpDuration = 1f;

    [Tooltip("Animation curve for the blend shape lerp.")]
    public AnimationCurve lerpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Meta Locomotion Reference")]
    [Tooltip("Reference to Meta's PlayerLocomotor, if you want to use TeleportInteractable logic.")]
    [SerializeField]
    private PlayerLocomotor metaLocomotor;

    // -------------------------------------------------------------------
    // PRIVATE FIELDS
    // -------------------------------------------------------------------

    private AudioSource audioSource;

    // -------------------------------------------------------------------
    // UNITY EVENTS
    // -------------------------------------------------------------------

    private void Start()
    {
        // Ensure the blendShapeObject has an AudioSource (optional)
        if (blendShapeObject != null)
        {
            audioSource = blendShapeObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("[PlayerMover] No AudioSource found on the blendShapeObject!");
            }
        }
        else
        {
            Debug.LogError("[PlayerMover] BlendShapeObject is not assigned!");
        }
    }

    // -------------------------------------------------------------------
    // PUBLIC METHODS
    // -------------------------------------------------------------------

    /// <summary>
    /// Initiates the sequence to move the player (via Meta's locomotion)
    /// and animate any door/elevator blend shapes.
    /// </summary>
    /// <param name="targetObject">The GameObject we want to move/teleport to.</param>
    public void MovePlayerToObject(GameObject targetObject)
    {
        // Basic null checks for references
        if (player == null)
        {
            Debug.LogError("[PlayerMover] Player GameObject is not assigned!");
            return;
        }

        if (blendShapeObject == null)
        {
            Debug.LogError("[PlayerMover] BlendShapeObject is not assigned!");
            return;
        }

        if (blendShapeTransformObject == null)
        {
            Debug.LogError("[PlayerMover] BlendShapeTransformObject is not assigned!");
            return;
        }

        if (targetObject == null)
        {
            Debug.LogError("[PlayerMover] Target GameObject is null!");
            return;
        }

        // (Optional) Play the audio clip from the blendShapeObject
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Start the coroutine that handles blend shapes + teleport
        StartCoroutine(MoveWithBlendShapeAndTransform(targetObject));
    }

    /// <summary>
    /// Instantly moves the blendShapeTransformObject to the position of the specified GameObject.
    /// </summary>
    /// <param name="targetObject">The GameObject to snap to.</param>
    public void MoveBlendShapeTransformObject(GameObject targetObject)
    {
        if (blendShapeTransformObject == null)
        {
            Debug.LogError("[PlayerMover] BlendShapeTransformObject is not assigned!");
            return;
        }

        if (targetObject == null)
        {
            Debug.LogError("[PlayerMover] Target GameObject is null!");
            return;
        }

        // Move it instantly
        blendShapeTransformObject.transform.position = targetObject.transform.position;
    }

    // -------------------------------------------------------------------
    // PRIVATE COROUTINE
    // -------------------------------------------------------------------

    /// <summary>
    /// Coroutine that:
    ///   1) Lerp the blend shape from 1 ? 0
    ///   2) Wait for 1 second
    ///   3) Teleport the player (via Meta's Locomotion if available)
    ///   4) Move the blendShapeTransformObject
    ///   5) Lerp the blend shape from 0 ? 1
    /// </summary>
    private IEnumerator MoveWithBlendShapeAndTransform(GameObject targetObject)
    {
        // 1) Lerp blend shape from 1 ? 0
        float elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            float t = elapsedTime / lerpDuration;
            float value = lerpCurve.Evaluate(t);
            blendShapeObject.SetBlendShapeWeight(blendShapeIndex, (1 - value) * 100f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blendShapeObject.SetBlendShapeWeight(blendShapeIndex, 0f);

        // 2) Wait for 1 second (optional)
        yield return new WaitForSeconds(1f);

        // 3) If we have a Meta PlayerLocomotor, create and dispatch a LocomotionEvent
        if (metaLocomotor != null)
        {
            // Attempt to retrieve TeleportInteractable from the target object
            TeleportInteractable teleportInteractable =
                targetObject.GetComponent<TeleportInteractable>();

            // Build the final pose 
            Pose finalPose;
            if (teleportInteractable != null)
            {
                // We pass Pose.identity as "hitPose" if there's no real arc data
                // The TeleportInteractable then decides final position/rotation (e.g. _targetPoint)
                finalPose = teleportInteractable.TargetPose(Pose.identity);
            }
            else
            {
                // Fallback if target doesn't have TeleportInteractable
                finalPose = new Pose(
                    targetObject.transform.position,
                    player.transform.rotation
                );
            }

            // Construct the LocomotionEvent with the correct signature:
            // (long identifier, Pose pose, TranslationType translation, RotationType rotation)
            const int IDENTIFIER = 0; // or any other unique long
            LocomotionEvent locoEvent = new LocomotionEvent(
                IDENTIFIER,
                finalPose,
                LocomotionEvent.TranslationType.Absolute, // or AbsoluteEyeLevel, depending on your needs
                LocomotionEvent.RotationType.None
            );

            // Dispatch the event to the locomotor
            metaLocomotor.HandleLocomotionEvent(locoEvent);
        }
        else
        {
            // If no PlayerLocomotor is assigned, fall back to manual movement
            player.transform.position = targetObject.transform.position;
        }

        // 4) Move the blendShapeTransformObject to the player's new position
        blendShapeTransformObject.transform.position = targetObject.transform.position;

        // Reset the timer for the second lerp
        elapsedTime = 0f;

        // 5) Lerp blend shape from 0 ? 1
        while (elapsedTime < lerpDuration)
        {
            float t = elapsedTime / lerpDuration;
            float value = lerpCurve.Evaluate(t);
            blendShapeObject.SetBlendShapeWeight(blendShapeIndex, value * 100f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blendShapeObject.SetBlendShapeWeight(blendShapeIndex, 100f);
    }
}
