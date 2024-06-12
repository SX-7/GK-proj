using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject player; // Reference to the player object
    public Vector3 movementVector; // The direction and amplitude of platform movement
    public float speed = 1.0f; // Speed at which the platform moves

    private Vector3 startPosition; // Initial position of the platform
    private Vector3 endPosition; // Calculated destination of the platform
    private float startTime; // Timestamp when movement starts
    private float journeyLength; // Total distance the platform needs to travel
    private Transform playerTransform; // Cache player's transform for efficiency
    private Vector3 playerOffset; // Relative position of player to the platform

    void Start()
    {
        // Initialize start position and calculate end position and journey length
        startPosition = transform.position;
        endPosition = startPosition + movementVector;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, endPosition);

        // Cache player's transform component if player is assigned
        if (player != null)
            playerTransform = player.transform;
    }

    void Update()
    {
        MovePlatform();

        // Maintain player's position relative to the moving platform
        if (playerTransform != null && playerTransform.parent == transform)
        {
            playerTransform.position = transform.position + playerOffset;
        }
    }

    void MovePlatform()
    {
        // Calculate the current position of the platform based on time
        float fractionOfJourney = (Time.time - startTime) * speed / journeyLength;
        transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.PingPong(fractionOfJourney, 1));
    }

    void OnCollisionEnter(Collision collision)
    {
        // Attach the player to the platform on collision
        if (collision.gameObject == player)
        {
            Debug.Log("Player entered platform");
            playerOffset = playerTransform.position - transform.position;
            playerTransform.SetParent(transform);

            // Temporarily disable gravity for a smoother experience on the platform
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Detach the player from the platform when they exit the collision
        if (collision.gameObject == player)
        {
            Debug.Log("Player exited platform");
            playerTransform.SetParent(null);

            // Re-enable gravity once the player leaves the platform
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
            }
        }
    }
}
