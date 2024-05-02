using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float delay = 0.5f;  // Time to wait before the platform starts falling
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();  // Ensure there is a Rigidbody to apply physics
            rb.isKinematic = true;  // Start kinematically to avoid falling without interaction
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))  // Check if the collider is the player
        {
            Invoke("StartFalling", delay);  // Call StartFalling after a delay
        }
    }

    void StartFalling()
    {
        rb.isKinematic = false;  // Enable physics response to gravity
        rb.useGravity = true;  // Ensure that gravity is enabled
        Destroy(gameObject, 5f);  // Optional: destroy the platform after 2 seconds to clean up
    }
}