using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyScript : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player game object not set. Please set the player game object in the inspector.");
            return;
        }

        UpdateChildrenMaterialAlpha();
    }

    void UpdateChildrenMaterialAlpha()
    {
        // Get all children
        foreach (Transform child in transform)
        {
            // Ensure the child object has a MeshRenderer component
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                // Loop over materials in the renderer
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    Material material = renderer.materials[i];
                    if (material.HasProperty("_Color"))
                    {
                        Color originalColor = material.color;
                        float distance = Vector3.Distance(player.transform.position, child.position);

                        // Ensure distance is not zero to avoid division by zero
                        if (distance > 0)
                        {
                            float alpha = 1 / distance;
                            // Clamp the alpha value to the range [0, 1]
                            alpha = Mathf.Clamp(alpha, 0f, 1f);
                            // Update the material color
                            material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                        }
                    }
                    else
                    {
                        Debug.LogError("Material on " + child.name + " does not have a _Color property");
                    }
                }
            }
            else
            {
                Debug.LogError("Child object " + child.name + " does not have a MeshRenderer component");
            }
        }
    }
}
