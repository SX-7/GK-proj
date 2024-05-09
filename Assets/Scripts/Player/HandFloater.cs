using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFloater : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    private Rigidbody playerRb;
    private Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.localPosition;
        controller = FindAnyObjectByType<PlayerController>();
        playerRb = controller.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //basically makes the hand do a little float around
        var destination = initPos + new Vector3(
                -controller.transform.InverseTransformDirection(playerRb.velocity).x / 500,
                -controller.transform.InverseTransformDirection(playerRb.velocity).y / 200,
                -controller.transform.InverseTransformDirection(playerRb.velocity).z / 500
            );
        transform.localPosition = Vector3.Lerp(transform.localPosition, destination, 0.02f);
        //Debug.Log(0.1f - (timer / (timerFreq * 10)));
    }
}