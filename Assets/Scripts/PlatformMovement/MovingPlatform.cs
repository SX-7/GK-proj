using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float height = 10f;  // Height the platform moves up
    public float speed = 1.0f;  // Movement speed
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float startTime;
    private float journeyLength;

    void Start()
    {
        startPosition = transform.position;  // Save the initial position as the start position
        endPosition = new Vector3(startPosition.x, startPosition.y + height, startPosition.z);  // Set end position based on the height
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, endPosition);
    }

    void Update()
    {
        // Calculate the current fraction of the journey completed based on time and speed
        float fractionOfJourney = (Time.time - startTime) * speed / journeyLength;
        // Use PingPong to smoothly oscillate between start and end positions
        transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.PingPong(fractionOfJourney, 1));
    }
}