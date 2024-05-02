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
    [SerializeField] SphereCollider sph;
    private float viewYAngle = 0f;
    private float viewXAngle = 0f;
    private List<Collider> vaultTargets = new List<Collider>();
    private bool climbing = false;
    [SerializeField] float climbingTime = 0.3f;
    private Vector3 origCamPos;
    private bool crouching = false;
    public delegate void CrouchAction(bool crouching);
    public static event CrouchAction OnCrouchChange;
    public delegate void JumpAction();
    public static event JumpAction OnJump;
    [SerializeField] float dashCooldown = 2f;
    [SerializeField] float dashDuration = 0.3f;
    [SerializeField] float dashForce = 50f;
    private float dashTimer = 0f;
    private bool dashing = false;
    public delegate void DashAction();
    public static event DashAction OnDash;
    // Start is called before the first frame update
    void Start()
    {
        //make sure we grab something
        if (rb == null) { rb = GetComponent<Rigidbody>(); }
        if (cam == null) { cam = GetComponentInChildren<Camera>(); }
        col = col != null ? col : GetComponent<CapsuleCollider>();
        if (sph == null) { sph = GetComponent<SphereCollider>(); }
        origCamPos = cam.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(dashTimer> 0f)
        {
            dashTimer-= Time.deltaTime;
        }
        //currently it's a very limited scenario, but if we are in an animation, we want to take control away from the player
        if (!(climbing||dashing))
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
                        x => Vector3.Distance(x, transform.position) < 2 * col.bounds.size.y
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

        if (Input.GetButton("Dash"))
        {
            Dash();
        }
    }

    private void CrouchingMode(bool crouch)
    {
        if (crouch)
        {
            if (!crouching)
            {
                crouching = true; 
                if(OnCrouchChange != null)
                {
                    OnCrouchChange(crouching);
                }
            }
            //We want no player movement input during sliding
            col.height = 0.95f;
            col.center = new Vector3(0, 0.6f, 0);
            sph.center = new Vector3(0,0.6f,0);
            //transform.localScale = new Vector3(1, 0.5f, 1);
            cam.transform.localPosition = origCamPos*0.5f;
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
                if (crouching)
                {
                    crouching = false;
                    if(OnCrouchChange!= null)
                    {
                        OnCrouchChange(crouching);
                    }
                }
                col.height = 1.9f;
                col.center = new Vector3(0, 1f, 0);
                sph.center = new Vector3(0, 0.5f, 0);
                cam.transform.localPosition = origCamPos;
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
        var des_hor_vel = new Vector3(leftright, 0, frontback);
        if (des_hor_vel.magnitude > 1)
        {
            des_hor_vel = des_hor_vel.normalized;
        }
        des_hor_vel *= speed;
        //var cur_hor_vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        //if ((Vector3.Dot(des_hor_vel, cur_hor_vel) < 0.8f || cur_hor_vel.magnitude<speed) && des_hor_vel.magnitude>0.1)
        //{
            rb.velocity = transform.TransformDirection(des_hor_vel + new Vector3(0, rb.velocity.y, 0));
        //}
        
    }

    private Vector3 GetClimbableVaultTarget(Collider other)
    {
        //finds a destination point above nearest climb plane
        var contact_point = other.transform.InverseTransformPoint(other.ClosestPointOnBounds(transform.position));
        contact_point.y = other.GetComponent<Climbable>().climbLevels.Where(x => x / other.transform.localScale.y >= contact_point.y).Min() / other.transform.localScale.y;
        var destination = other.transform.TransformPoint(contact_point);
        return destination;
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

    private void Dash()
    {
        if(dashTimer<=0f)
        {
            dashTimer = dashCooldown;
            if (OnDash != null)
            {
                OnDash();
            }
            StartCoroutine(DashLockout());
        }   
    }
    private IEnumerator DashLockout()
    {
        dashing = true;
        for (float i = dashTimer; i > (dashCooldown - dashDuration); i = dashTimer)
        {
            rb.velocity = cam.transform.forward * dashForce;
            yield return null;
        }
        rb.velocity = rb.velocity.normalized * speed;
        dashing = false;
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        if (OnJump != null)
        {
            OnJump();
        }
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
        rb.velocity = (destination - midpoint).normalized * speed *1.5f;
    }
}