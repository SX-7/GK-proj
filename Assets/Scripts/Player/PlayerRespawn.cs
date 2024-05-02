using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 startPosition;  // Initial position for respawn reference

    void Start()
    {
        // Initialize startPosition with the character's original position at scene start
        startPosition = transform.position;
    }

    void Update()
    {
        // Continuously check for a fall beyond the respawn threshold
        if (transform.position.y < -20)  // Threshold for falling, adjust as needed
        {
            Respawn();
        }
    }

    void Respawn()
    {
        // Move the character to the starting position and reset dynamics
        transform.position = startPosition;
        ResetDynamics();
    }

    private void ResetDynamics()
    {
        // Reset velocity to prevent accumulated motion effects
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }
}
