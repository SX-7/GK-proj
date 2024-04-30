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
    private List<Collider> vaultTargets = new List<Collider>();
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
            if (OnWalkable())
            {
                Jump();
            }
            else
            {
                if (vaultTargets.Count > 0)
                {
                    var positions = vaultTargets.Select(
                        (x,index) => GetClimbableVaultTarget(x)
                        ).Where(
                        x => Vector3.Distance(x,transform.position) < col.bounds.size.y
                        ).ToList();
                    if (positions.Count>0)
                    {
                        transform.position = positions.First();
                    }
                }
            }
        }
        //cam.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));
        //cam.transform.Rotate(cam.transform.right, Input.GetAxis("Mouse Y"));
        RotateCamera(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (Input.GetButton("Crouch"))
        {
            //We want no player movement input during sliding
            transform.localScale = new Vector3(1, 0.5f, 1);
        } else {
            transform.localScale = new Vector3(1, 1, 1);
            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Climbable>())
        {
            vaultTargets.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Climbable>())
        {
            vaultTargets.Remove(other);
        }
    }

    private void Move(float leftright, float frontback)
    {
        rb.velocity = transform.TransformDirection(new Vector3(leftright, 0, frontback) * speed + new Vector3(0, rb.velocity.y, 0));
    }

    private Vector3 GetClimbableVaultTarget(Collider other)
    {
        var contact_point = other.transform.InverseTransformPoint(other.ClosestPointOnBounds(transform.position));
        contact_point.y = other.GetComponent<Climbable>().offsets.Where(x => x / other.transform.localScale.y >= contact_point.y).Min() / other.transform.localScale.y;
        var destination = other.transform.TransformPoint(contact_point);
        return destination + new Vector3(0, col.bounds.size.y / 2, 0);
    }

    private bool OnWalkable()
    {
        return Physics.RaycastAll(
                    transform.TransformPoint(col.center) - new Vector3(0, col.bounds.size.y / 3, 0),
                    Vector3.down,
                    0.5f
                ).Where(
                    x => x.transform.GetComponent<Walkable>() != null
                ).ToList().Count > 0;
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    private void RotateCamera(float x_delta, float y_delta)
    {
        viewYAngle -= y_delta;
        viewYAngle = Mathf.Clamp(viewYAngle, -80, 80);
        viewXAngle += x_delta;
        cam.transform.eulerAngles = new Vector3(viewYAngle, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, viewXAngle, transform.eulerAngles.z);
    }
}
