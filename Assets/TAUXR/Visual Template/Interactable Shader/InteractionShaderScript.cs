using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionShaderScript : MonoBehaviour
{
    public bool SendHAndPositions = true;
    public bool SendPinchStrengths =true; 
    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }
        else
        {
            Debug.LogError("Renderer component not found on this GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {

        // If SendPinch, it sends the position of the pinch point and strengths
        // else it sendes Pinch strength as 1
        
        if (material != null)
        {
            if (SendPinchStrengths)
            {
                material.SetFloat("_RightHandPinch", TXRPlayer.Instance.HandRight.PinchManager.Pincher.Strength);
                material.SetFloat("_LeftHandPinch", TXRPlayer.Instance.HandLeft.PinchManager.Pincher.Strength);
                material.SetVector("_RightHandPosition", TXRPlayer.Instance.HandRight.PinchManager.Pincher.transform.position);
                material.SetVector("_LeftHandPosition", TXRPlayer.Instance.HandLeft.PinchManager.Pincher.transform.position);
            }

            else if (SendHAndPositions)
            {
                material.SetVector("_RightHandPosition", TXRPlayer.Instance.HandRight.PinchManager.Pincher.transform.position);
                material.SetVector("_LeftHandPosition", TXRPlayer.Instance.HandLeft.PinchManager.Pincher.transform.position);
                material.SetFloat("_RightHandPinch", 1f);
                material.SetFloat("_LeftHandPinch", 1f);
            }

            
        }
    }
}
