using UnityEngine;
using UnityEngine.SceneManagement; // Potrzebne do ładowania scen

public class ResetOnTouch : MonoBehaviour
{
    // Nazwa sceny, do której chcesz przenieść gracza
    public string sceneName = "DemoLevel_v1";

    private void OnTriggerEnter(Collider other)
    {
        // Sprawdź, czy obiekt, który wszedł w kolizję to gracz
        if (other.CompareTag("Player"))
        {
            // Załaduj scenę początkową
            SceneManager.LoadScene(sceneName);
        }
    }
}
