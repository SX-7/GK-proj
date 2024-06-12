using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    private PlayerController player;
    private Vector3 normalisedPos;
    // Start is called before the first frame update
    void Start()
    {
        player = FindAnyObjectByType(typeof(PlayerController)) as PlayerController;
        normalisedPos = new Vector3(transform.position.x,0,transform.position.z);
        SetScore(PlayerController.Score);
    }

    // Update is called once per frame
    void Update()
    {
        var p_normalised_pos = new Vector3(player.transform.position.x, 0 ,player.transform.position.z);
        transform.rotation = Quaternion.LookRotation(normalisedPos - p_normalised_pos);
        if (!PlayerController.Won)
        {
            SetScore(Random.Range(-10000,-1000));
        }
    }

    void SetScore(float score)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = "Score: " + ((int)score).ToString();
    }
}
