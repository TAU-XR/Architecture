using UnityEngine;
using System.Collections.Generic;

// Define the entry class before the manager class.
[System.Serializable]
public class HeightToggleEntry
{
    [Tooltip("The object to toggle on/off")]
    public GameObject targetObject;

    [Tooltip("Minimum head height for the object to be active")]
    public float minHeight;

    [Tooltip("Maximum head height for the object to be active")]
    public float maxHeight;

    [HideInInspector]
    public bool isWithinThreshold;
}

public class HeightToggleManager : MonoBehaviour
{
    [Tooltip("List of objects with their corresponding height thresholds")]
    public List<HeightToggleEntry> toggleEntries = new List<HeightToggleEntry>();

    private void Update()
    {
        // Ensure the VR player's head is available.
        // Note: PlayerHead is a property, so access it without parentheses.
        if (TXRPlayer.Instance == null || TXRPlayer.Instance.PlayerHead == null)
            return;

        // Get the player's head height.
        float headHeight = TXRPlayer.Instance.PlayerHead.transform.position.y;

        // Loop through each entry to update the object's active state.
        foreach (HeightToggleEntry entry in toggleEntries)
        {
            // Check if the head height is within the specified thresholds.
            bool inThreshold = (headHeight >= entry.minHeight && headHeight <= entry.maxHeight);

            // Only update if there is a change (entering or exiting the threshold).
            if (inThreshold != entry.isWithinThreshold)
            {
                if (entry.targetObject != null)
                {
                    entry.targetObject.SetActive(inThreshold);
                }
                entry.isWithinThreshold = inThreshold;
            }
        }
    }
}
