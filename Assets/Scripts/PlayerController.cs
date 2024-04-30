using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float jumpHeight = 10f;
    private Rigidbody rb;
    private Camera cam;
    private float viewYAngle = 0f;
    private float viewXAngle = 0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
        //cam.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));
        //cam.transform.Rotate(cam.transform.right, Input.GetAxis("Mouse Y"));
        viewYAngle -= Input.GetAxis("Mouse Y");
        viewYAngle = Mathf.Clamp(viewYAngle, -80, 80);
        viewXAngle += Input.GetAxis("Mouse X");
        cam.transform.eulerAngles = new Vector3(viewYAngle, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, viewXAngle, transform.eulerAngles.z);
        rb.velocity = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed + new Vector3(0, rb.velocity.y, 0));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Climbable>())
        {
            
        }
    }
}
