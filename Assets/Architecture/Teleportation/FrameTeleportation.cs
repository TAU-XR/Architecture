using UnityEngine;

public class FrameShaderUpdater : MonoBehaviour
{
    public GameObject frame;  // The frame around the player
    public Transform player;  // The player or player rig
    private Material frameMaterial;   // Material of the frame
    private static readonly int PlayerPositionID = Shader.PropertyToID("_Player_Position");

    private void Start()
    {
        // Get the frame's material
        if (frame != null)
        {
            Renderer frameRenderer = frame.GetComponent<Renderer>();
            if (frameRenderer != null)
            {
                frameMaterial = frameRenderer.material;
            }
        }
    }

    private void Update()
    {
        // Update shader with player's current position every frame
        UpdateShaderWithPlayerPosition();
    }

    private void UpdateShaderWithPlayerPosition()
    {
        if (frameMaterial != null && player != null)
        {
            // Update the shader variable "_Player_Position" with the player's current position
            frameMaterial.SetVector(PlayerPositionID, TXRPlayer.Instance.PlayerHead.transform.position);
        }
    }
}
