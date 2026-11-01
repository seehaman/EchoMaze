using UnityEngine;

// Keeps the camera following the player at a fixed offset (from the Roll a Ball lecture).
public class CameraController : MonoBehaviour
{
    // Reference to the player GameObject. Drag the Player here in the Inspector.
    public GameObject player;

    // The distance between the camera and the player.
    private Vector3 offset;

    void Start()
    {
        // Calculate the starting offset between the camera's position and the player's position.
        offset = transform.position - player.transform.position;
    }

    // LateUpdate runs after all Update functions, so the camera moves after the player.
    void LateUpdate()
    {
        // Keep the same offset between the camera and player for the whole game.
        transform.position = player.transform.position + offset;
    }
}
