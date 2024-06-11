using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int victoryPoints = 0;
    private int pointsToWin = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddVictoryPoint()
    {
        victoryPoints++;
        Debug.Log("Victory Points: " + victoryPoints);

        if (victoryPoints >= pointsToWin)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        Debug.Log("You Win!");

    }
}