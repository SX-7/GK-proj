using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 90f;  // Rotation speed in degrees per second

    void Update()
    {
        // Rotate the object around its local Y-axis at the specified rotation speed
        transform.RotateAround(transform.position, transform.up, rotationSpeed * Time.deltaTime);
    }
}
