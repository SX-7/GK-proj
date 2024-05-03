using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum MovementDirection { Horizontal, Vertical }  // Options for movement direction
    public MovementDirection direction = MovementDirection.Horizontal;  // Default movement direction
    public float moveDistance = 10f;  // Distance the platform moves from the start position
    public float speed = 1.0f;  // Movement speed

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float startTime;
    private float journeyLength;

    void Start()
    {
        startPosition = transform.position;
        SetMovementDirection();
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, endPosition);
    }

    void Update()
    {
        float fractionOfJourney = (Time.time - startTime) * speed / journeyLength;
        transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.PingPong(fractionOfJourney, 1));
    }

    private void SetMovementDirection()
    {
        // Set end position based on the selected movement direction
        if (direction == MovementDirection.Horizontal)
        {
            endPosition = startPosition + Vector3.right * moveDistance;
        }
        else if (direction == MovementDirection.Vertical)
        {
            endPosition = startPosition + Vector3.up * moveDistance;
        }
    }
}
