using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int victoryPoints = 0;
    public int pointsToWin = 1;
    private PlayerController playerController;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerController = playerObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Debug.Log("FIND PLAYER COMPONENT");
            }
            else
            {
                Debug.LogError("CANT FIND PLAYER COMPONENT");
            }
        }
    }

    public void AddVictoryPoint()
    {
        victoryPoints++;
        CheckVictoryPoints();
    }

    void CheckVictoryPoints()
    {
        if (victoryPoints >= pointsToWin)
        {
            VictoryAchieved();
        }
    }

    void VictoryAchieved()
    {
        Debug.Log("Victory achieved! You have collected " + victoryPoints + " points.");
        PlayerController.Won = true;
        playerController.SendMessage("Death");

    }
}
