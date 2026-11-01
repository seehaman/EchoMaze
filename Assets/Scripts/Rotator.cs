using UnityEngine;

// Spins an object all the time (from the Roll a Ball lecture).
// Used on the spinners and on the goal.
public class Rotator : MonoBehaviour
{
    // Degrees per second around X, Y and Z. Spinners only use Y.
    public Vector3 rotationSpeed = new Vector3(15, 30, 45);

    void Update()
    {
        // Time.deltaTime makes the speed the same on fast and slow computers.
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
