using System.Collections; // Required for IEnumerator
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    // Reference to the Player GameObject
    public GameObject player;

    // Reference to the SkinnedMeshRenderer with the blendshape
    public SkinnedMeshRenderer blendShapeObject;

    // Reference to the GameObject that will move along with the Player
    public GameObject blendShapeTransformObject;

    // Blendshape index (default is "Key 1")
    public int blendShapeIndex = 0;

    // Duration for the lerp
    public float lerpDuration = 1f;

    // Animation curve for the lerp
    public AnimationCurve lerpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Audio source for the sound effect
    private AudioSource audioSource;

    private void Start()
    {
        // Ensure the blendShapeObject has an AudioSource
        if (blendShapeObject != null)
        {
            audioSource = blendShapeObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("No AudioSource found on the blendShapeObject!");
            }
        }
        else
        {
            Debug.LogError("BlendShapeObject is not assigned!");
        }
    }

    /// <summary>
    /// Moves the player and blend shape object to the position of the target GameObject with blendshape lerping.
    /// </summary>
    /// <param name="targetObject">The GameObject whose position the player will move to.</param>
    public void MovePlayerToObject(GameObject targetObject)
    {
        if (player == null)
        {
            Debug.LogError("Player GameObject is not assigned!");
            return;
        }

        if (blendShapeObject == null)
        {
            Debug.LogError("Blendshape object is not assigned!");
            return;
        }

        if (blendShapeTransformObject == null)
        {
            Debug.LogError("Blend shape transform object is not assigned!");
            return;
        }

        if (targetObject == null)
        {
            Debug.LogError("Target GameObject is null!");
            return;
        }

        // Play the audio clip from the blendShapeObject
        if (audioSource != null)
        {
            audioSource.Play();
        }

        StartCoroutine(MoveWithBlendShapeAndTransform(targetObject));
    }

    /// <summary>
    /// Instantly moves the blendShapeTransformObject to the position of the specified GameObject.
    /// </summary>
    /// <param name="targetObject">The target GameObject to move the blendShapeTransformObject to.</param>
    public void MoveBlendShapeTransformObject(GameObject targetObject)
    {
        if (blendShapeTransformObject == null)
        {
            Debug.LogError("BlendShapeTransformObject is not assigned!");
            return;
        }

        if (targetObject == null)
        {
            Debug.LogError("Target GameObject is null!");
            return;
        }

        blendShapeTransformObject.transform.position = targetObject.transform.position;
    }

    /// <summary>
    /// Coroutine to handle the blendshape lerp and synchronized movement.
    /// </summary>
    /// <param name="targetObject">The GameObject whose position the player will move to.</param>
    private IEnumerator MoveWithBlendShapeAndTransform(GameObject targetObject)
    {
        float elapsedTime = 0f;

        // Lerp blendshape from 1 to 0
        while (elapsedTime < lerpDuration)
        {
            float t = elapsedTime / lerpDuration;
            float value = lerpCurve.Evaluate(t);
            blendShapeObject.SetBlendShapeWeight(blendShapeIndex, (1 - value) * 100f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blendShapeObject.SetBlendShapeWeight(blendShapeIndex, 0f);

        // Wait for 1 second after the first lerp
        yield return new WaitForSeconds(1f);

        // Move the player to the target position
        player.transform.position = targetObject.transform.position;

        // Move the blendShapeTransformObject to the same position as the player
        blendShapeTransformObject.transform.position = player.transform.position;

        // Reset elapsed time for second lerp
        elapsedTime = 0f;

        // Lerp blendshape from 0 to 1
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
