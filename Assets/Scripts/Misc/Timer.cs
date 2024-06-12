using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshPro textMeshPro;
    private float current_time = 0f;
    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = textMeshPro != null ? textMeshPro : GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        current_time += Time.deltaTime;
        var time = TimeSpan.FromSeconds(current_time);
        textMeshPro.text = time.Seconds+"."+time.Milliseconds;
        if (time.Minutes > 0)
        {
            textMeshPro.text = time.Minutes + ":" + textMeshPro.text;
        }
        if (time.Hours > 0)
        {
            textMeshPro.text = time.Hours+":"+textMeshPro.text;
        }
    }
}
