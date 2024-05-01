using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vial : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;
    private void OnEnable()
    {
        //PlayerController.OnHealthChange += OnHealthChange;

    }

    private void OnDisable()
    {
        //PlayerController.OnHealthChange -= OnHealthChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime),Space.Self);
    }
}
