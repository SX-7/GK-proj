using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float jumpHeight = 10f;
    [SerializeField] Rigidbody rb;
    [SerializeField] Camera cam;
    [SerializeField] CapsuleCollider col;
    private float viewYAngle = 0f;
    private float viewXAngle = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if (rb == null) { rb = GetComponent<Rigidbody>(); }
        if (cam == null) { cam = GetComponentInChildren<Camera>(); }
        if (col == null) { col = GetComponent<CapsuleCollider>(); }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if(
                Physics.RaycastAll(
                    transform.TransformPoint(col.center) - new Vector3(0, col.bounds.size.y / 3, 0), 
                    Vector3.down, 
                    0.5f
                ).Where(
                    x => x.transform.GetComponent<Walkable>() != null
                ).ToList().Count > 0
            )
            {
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            }
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

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Climbable>())
        {
            if (Input.GetButton("Jump"))
            {
                var contact_point = other.transform.InverseTransformPoint(other.ClosestPointOnBounds(transform.position));
                Debug.Log(other.ClosestPointOnBounds(transform.position));
                contact_point.y = other.GetComponent<Climbable>().offsets.Where(x => x / other.transform.localScale.y >= contact_point.y).Min() / other.transform.localScale.y;
                var destination = other.transform.TransformPoint(contact_point);
                Debug.Log(destination);
                if (Vector3.Distance(destination, transform.position - new Vector3(0, col.bounds.size.y / 2, 0)) < col.bounds.size.y)
                {
                    transform.position = destination + new Vector3(0, col.bounds.size.y / 2, 0);
                }
            }
        }
    }
}
