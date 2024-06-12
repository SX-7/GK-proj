using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float knockbackStrength = 10f; // Siła knockbacku

    private void OnCollisionEnter(Collision collision)
    {
        // Sprawdzenie, czy obiekt z którym doszło do kolizji ma tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Obliczenie kierunku knockbacku
                Vector3 knockbackDirection = collision.transform.position - transform.position;
                knockbackDirection.Normalize();

                // Zastosowanie siły knockbacku
                rb.AddForce(knockbackDirection * knockbackStrength, ForceMode.Impulse);
            }
        }
    }
}
