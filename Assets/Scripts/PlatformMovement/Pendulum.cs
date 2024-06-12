using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public float speed = 1.5f;
    public float limit = 75f;
    public bool randomStart = false;
    public float knockbackStrength = 50f; // Siła knockbacku
    private float random = 0;

    private Rigidbody rb;

    void Awake()
    {
        if (randomStart)
        {
            random = Random.Range(0f, 1f);
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Make it kinematic to control its motion manually
        }
    }

    void Update()
    {
        float angle = limit * Mathf.Sin((Time.time + random) * speed);
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnCollisionEnter(Collision collision)
    {

        // Sprawdzenie, czy obiekt z którym doszło do kolizji ma tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {

            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Obliczenie kierunku knockbacku
                Vector3 knockbackDirection = collision.transform.position - transform.position;
                knockbackDirection.y = 0; // Ignorujemy knockback w osi Y, jeśli nie chcemy pionowego ruchu
                knockbackDirection.Normalize();

                // Uwzględnienie prędkości kuli w knockbacku
                Vector3 knockbackForce = knockbackDirection * knockbackStrength + rb.velocity;
                playerRb.AddForce(knockbackForce, ForceMode.Impulse);

            }
        }
    }
}
