using UnityEngine;

public class HurtSound : MonoBehaviour
{
    public AudioClip hurtClip;  // Dźwięk bólu
    public float volume = 1.0f; // Głośność dźwięku
    public float delay = 0.5f;  // Opóźnienie między dźwiękami w sekundach
    private AudioSource audioSource;
    private float lastPlayTime;

    void Start()
    {
        // Dodaj komponent AudioSource do obiektu, jeśli nie istnieje
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Ustawienia AudioSource
        audioSource.clip = hurtClip;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
        lastPlayTime = -delay; // Inicjalizacja lastPlayTime
    }

    // Wykryj kolizję z graczem
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayHurtSound();
        }
    }

    // Funkcja odtwarzająca dźwięk bólu
    void PlayHurtSound()
    {
        if (Time.time - lastPlayTime >= delay)
        {
            audioSource.Play();
            lastPlayTime = Time.time;
        }
    }
}
