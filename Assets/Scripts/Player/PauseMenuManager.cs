using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject pointer;
    private Vector3 designated;
    // Start is called before the first frame update
    void Start()
    {
        designated = pointer.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        pointer.transform.position = Vector3.Lerp(designated, pointer.transform.position, 0.9f);
    }

    private void OnEnable()
    {
        pointer.transform.position += pointer.transform.TransformDirection(new Vector3(20, 0, 0));
        designated = pointer.transform.position;
        pointer.SetActive(false);
    }

    private void ItemSelected(Vector3 position)
    {
        if (!pointer.activeSelf)
        {
            pointer.SetActive(true);
            pointer.transform.position = position - pointer.transform.TransformDirection(new Vector3(0, 0, 0.1f));
        }
        designated = position - pointer.transform.TransformDirection(new Vector3(0, 0, 0.1f));
    }
}
