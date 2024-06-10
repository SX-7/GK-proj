using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vial : MonoBehaviour
{
    [SerializeField] VisualsData visualsData;
    private float RotationSpeed { get => visualsData.vialRotationSpeed; }
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, RotationSpeed * Time.deltaTime),Space.Self);
    }
}
