using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject player;
    public Vector3 movementVector;
    public float speed = 1.0f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float startTime;
    private float journeyLength;

    void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + movementVector;  // Calculate the end position
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, endPosition);
    }

    void Update()
    {
        MovePlatform();
    }

    void MovePlatform()
    {
        float fractionOfJourney = (Time.time - startTime) * speed / journeyLength;
        Vector3 newPlatformPosition = Vector3.Lerp(startPosition, endPosition, Mathf.PingPong(fractionOfJourney, 1));
        transform.position = newPlatformPosition;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            collision.transform.SetParent(transform, true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            collision.transform.SetParent(null);
        }
    }
}
