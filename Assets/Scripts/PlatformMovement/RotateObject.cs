using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up;  // Axis around which the object will rotate
    public float rotationSpeed = 90.0f;  // Speed of rotation in degrees per second

    void Start()
    {
        // Generate a random initial rotation between 0 and 360 degrees
        float initialRotation = Random.Range(0f, 360f);

        // Apply the initial rotation offset
        transform.Rotate(rotationAxis, initialRotation);
    }

    void Update()
    {
        // Rotate around the specified axis at 'rotationSpeed' degrees per second.
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
