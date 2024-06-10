using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpScaler : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] VisualsData visualData;
    private float HealthLerpSpeed { get => visualData.healthLerpSpeed; }

    // Update is called once per frame
    void Update()
    {
        var ls = transform.localScale;
        ls.z =Mathf.Lerp(Mathf.Clamp01(playerController.CurrentHealth / playerController.MaxHealth), transform.localScale.z, HealthLerpSpeed);
        transform.localScale = ls;
    }
}
