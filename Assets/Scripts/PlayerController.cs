using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Moves the player ball (same idea as the Roll a Ball lecture) and records
// every position it passes through, so a failed run can become an Echo.
public class PlayerController : MonoBehaviour
{
    // Speed at which the player moves. Change it in the Inspector.
    public float speed = 12;

    // Drag the GameManager object here in the Inspector.
    public GameManager gameManager;

    // All the positions of the current run. When the player dies,
    // the GameManager gives this list to a new Echo.
    public List<Vector3> recordedPath = new List<Vector3>();

    // Rigidbody of the player.
    private Rigidbody rb;

    // Movement along X and Y axes (from the keyboard).
    private float movementX;
    private float movementY;

    void Start()
    {
        // Get and store the Rigidbody component attached to the player.
        rb = GetComponent<Rigidbody>();
    }

    // Called automatically by the Player Input component when WASD / arrows are pressed.
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void FixedUpdate()
    {
        // Do nothing on the start screen, after dying, or after winning.
        if (!gameManager.isPlaying) return;

        // Push the ball in the direction of the keys (Roll a Ball movement).
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);

        // Remember where the ball is on this physics step.
        recordedPath.Add(transform.position);
    }

    // Runs when the ball touches a collider that has "Is Trigger" ticked.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            // Spikes, spinner arms and Echoes all use the "Hazard" tag.
            gameManager.PlayerDied(other.gameObject.name);
        }
        else if (other.gameObject.CompareTag("Goal"))
        {
            gameManager.LevelComplete();
        }
    }

    // Puts the ball back at the start and begins a new recording.
    public void ResetToStart(Vector3 startPosition)
    {
        transform.position = startPosition;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Make a NEW list: the old one now belongs to an Echo.
        recordedPath = new List<Vector3>();
    }
}
