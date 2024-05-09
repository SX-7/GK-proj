using UnityEngine;

public class TrampolinePlatform : MonoBehaviour
{
    public float bounceStrength = 20f;  // Adjust this value to increase or decrease the bounce effect

    private void OnCollisionEnter(Collision collision)
    {
        // Ensure the object is moving downwards before applying the bounce force
        if (collision.relativeVelocity.y <= 0f)
        {
            Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Impulse force is used here for an instantaneous bounce effect
                rb.AddForce(Vector3.up * bounceStrength, ForceMode.Impulse);
            }
        }
    }
}
