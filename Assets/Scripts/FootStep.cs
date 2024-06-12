using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioClip footstepClip; // Plik dźwiękowy kroku
    private AudioSource audioSource; // Komponent AudioSource

    void Start()
    {
        // Dodajemy komponent AudioSource do obiektu, jeśli go nie ma
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = footstepClip;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Sprawdzenie czy obiekt kolidujący to gracz (można zmodyfikować zgodnie z potrzebami)
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayFootstepSound();
        }
    }

    void PlayFootstepSound()
    {
        // Odtwarzanie dźwięku kroku
        if (audioSource != null && footstepClip != null)
        {
            audioSource.PlayOneShot(footstepClip);
        }
    }
}
