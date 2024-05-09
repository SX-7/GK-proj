using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodColorer : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Gradient gradient;
    // Start is called before the first frame update
    void Start()
    {
        if (gradient == null) { throw new UnassignedReferenceException("No gradient assigned to " + name); }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(playerController.CurrentHealth / playerController.MaxHealth);
        GetComponent<Renderer>().material.SetColor("_Color",gradient.Evaluate(playerController.CurrentHealth / playerController.MaxHealth));
        GetComponent<Renderer>().material.SetColor("_EmissionColor", gradient.Evaluate(playerController.CurrentHealth / playerController.MaxHealth));
    }
}
