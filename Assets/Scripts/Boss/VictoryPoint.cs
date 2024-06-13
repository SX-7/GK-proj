using UnityEngine;

public class VictoryPoint : MonoBehaviour
{
    private PlayerController playerController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            Debug.Log("VC: " + other.gameObject.name);
            
            GameManager.instance.AddVictoryPoint();

            Destroy(gameObject); 
        }
    }
}
