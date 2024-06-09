using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodColorer : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] VisualsData visualData;
    private Gradient Gradient { get => visualData.healthColorGradient; }
    // Start is called before the first frame update
    void Start()
    {
        if (Gradient == null) { throw new UnassignedReferenceException("No gradient assigned to " + name); }
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().material.SetColor("_Color",Gradient.Evaluate(playerController.CurrentHealth / playerController.MaxHealth));
        GetComponent<Renderer>().material.SetColor("_EmissionColor", Gradient.Evaluate(playerController.CurrentHealth / playerController.MaxHealth) * Mathf.LinearToGammaSpace(50f));
        GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
    }
}
