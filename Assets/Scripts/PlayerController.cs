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
    private bool climbing = false;
    [SerializeField] float climbingTime = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        //make sure we grab something
        if (rb == null) { rb = GetComponent<Rigidbody>(); }
        if (cam == null) { cam = GetComponentInChildren<Camera>(); }
        if (col == null) { col = GetComponent<CapsuleCollider>(); }
    }

    // Update is called once per frame
    void Update()
    {
        //currently it's a very limited scenario, but if we are in an animation, we want to take control away from the player
        if (!climbing)
        {
            ProcessMovement();
        }
        RotateCamera(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void ProcessMovement()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (OnWalkable())
            {
                Jump();
            }
            else
            {
                // Check for possible vaulting
                if (vaultTargets.Count > 0)
                {
                    var positions = vaultTargets.Select(
                        (x, index) => GetClimbableVaultTarget(x)
                        ).Where(
                        x => Vector3.Distance(x, transform.position) < col.bounds.size.y
                        ).ToList();
                    if (positions.Count > 0)
                    {
                        Climb(positions.First());
                    }
                }
            }
        }
        //cam.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));
        //cam.transform.Rotate(cam.transform.right, Input.GetAxis("Mouse Y"));

        if (Input.GetButton("Crouch"))
        {
            CrouchingMode(true);
        }
        else
        {
            //it is possible to stop crouch when you can't physically uncrouch, due to no limited friction during crouch, we allow movement to avoid softlocks
            CrouchingMode(false);
            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    private void CrouchingMode(bool crouch)
    {
        if (crouch)
        {
            //We want no player movement input during sliding
            transform.localScale = new Vector3(1, 0.5f, 1);
            cam.transform.position = transform.position + new Vector3(0, transform.localScale.y, 0);
        }
        else
        {
            //check if we can uncrouch
            if (Physics.RaycastAll(
                    transform.TransformPoint(col.center) + new Vector3(0, col.bounds.size.y / 3, 0),
                    Vector3.up,
                    col.bounds.size.y
                ).Where(
                    x => x.transform.GetComponent<PlayerController>() == null
                ).ToList().Count == 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                cam.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            }

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //Track availible vault targets
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
        //finds a destination point above nearest climb plane
        var contact_point = other.transform.InverseTransformPoint(other.ClosestPointOnBounds(transform.position));
        contact_point.y = other.GetComponent<Climbable>().climbLevels.Where(x => x / other.transform.localScale.y >= contact_point.y).Min() / other.transform.localScale.y;
        var destination = other.transform.TransformPoint(contact_point);
        return destination + new Vector3(0, col.bounds.size.y / 2, 0);
    }

    private bool OnWalkable()
    {
        //test if object with walkable is *right* below us (we're standing on it)
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
        //helper function
        viewYAngle -= y_delta;
        viewYAngle = Mathf.Clamp(viewYAngle, -80, 80);
        viewXAngle += x_delta;
        cam.transform.eulerAngles = new Vector3(viewYAngle, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, viewXAngle, transform.eulerAngles.z);
    }

    private void Climb(Vector3 destination)
    {
        // calculate path (up then forward)
        var midpoint = new Vector3(transform.position.x, destination.y, transform.position.z);
        // start coroutine with path 
        StartCoroutine(ClimbCR(midpoint, destination));
    }

    private IEnumerator ClimbCR(Vector3 midpoint, Vector3 destination)
    {
        climbing = true;
        rb.velocity = Vector3.zero;
        //we attempt to finish the animation in one constant motion. might be changed later
        var anim_time = climbingTime;
        var start = transform.position;
        var to_mid_len = Vector3.Distance(start, midpoint);
        var to_end_len = Vector3.Distance(midpoint, destination);
        var to_mid_time = anim_time * to_mid_len / (to_mid_len + to_end_len);
        var to_end_time = anim_time * to_end_len / (to_mid_len + to_end_len);
        for (float i = 0; i < to_mid_time; i += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(start, midpoint, i / to_mid_time);
            yield return null;
        }
        for (float i = 0; i < to_end_time; i += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(midpoint, destination, i / to_end_time);
            yield return null;
        }
        climbing = false;
        rb.velocity = (destination - midpoint).normalized * 10f;
    }
}
