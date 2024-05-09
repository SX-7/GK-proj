using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static ActionItem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float maxSpeed = 10f;
    public float MaxSpeed { get => maxSpeed; }
    [SerializeField] float jumpHeight = 10f;
    [SerializeField] float jumpCooldown = 0.1f;
    private float jumpTimer = 0.0f;
    [SerializeField] float maxHealth = 100;
    public float MaxHealth { get => maxHealth; }
    [SerializeField] float currentHealth = 100;
    public float CurrentHealth { get => currentHealth; }
    [SerializeField] float iFrames = 1f;
    public float IFrames { get => iFrames; }
    private float iFrameTimer = 0f;
    public delegate void ReceiveDamageAction(DamageInfo damageInfo);
    public static event ReceiveDamageAction OnReceiveDamage;
    [SerializeField] Rigidbody rb;
    [SerializeField] Camera cam;
    [SerializeField] CapsuleCollider col;
    [SerializeField] SphereCollider sph;
    [SerializeField] Vector3 respawnPosition;
    private float viewYAngle = 0f;
    private float viewXAngle = 0f;
    private List<Collider> vaultTargets = new();
    private bool climbing = false;
    [SerializeField] float climbingTime = 0.3f;
    private Vector3 origCamPos;
    private bool crouching = false;
    public delegate void CrouchAction(bool crouching);
    public static event CrouchAction OnCrouchChange;
    public delegate void JumpAction();
    public static event JumpAction OnJump;
    [SerializeField] float dashCooldown = 2f;
    public float DashCooldown { get => dashCooldown; }
    [SerializeField] float dashDuration = 0.3f;
    public float DashDuration { get => dashDuration; }
    [SerializeField] float dashForce = 50f;
    [SerializeField] int maxDashes = 3;
    public int MaxDashes { get => maxDashes; }
    public Vector3 Velocity { get => rb.velocity; }
    private float dashTimer = 0f;
    private int currentDashes = 0;
    public int CurrentDashes { get => currentDashes; }
    private bool dashing = false;
    public delegate void DashAction(int currentDashes);
    public static event DashAction OnDash;
    public delegate void DashRestoreAction(int currentDashes);
    public static event DashRestoreAction OnDashRestore;
    public delegate void SlowMotionStateChange(bool enabled);
    public static event SlowMotionStateChange OnSlowMotion;
    private InputBuffer inputs = new();
    private bool slowing = false;
    [SerializeField] float coyoteJumpTime = 0.12f;
    private float cJumpTimer = 0f;
    // Start is called before the first frame update

    private void Awake()
    {
        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
    }
    void Start()
    {
        //make sure we grab something
        if (rb == null) { rb = GetComponent<Rigidbody>(); }
        if (cam == null) { cam = GetComponentInChildren<Camera>(); }
        col = col != null ? col : GetComponent<CapsuleCollider>();
        if (sph == null) { sph = GetComponent<SphereCollider>(); }
        origCamPos = cam.transform.localPosition;
        if (respawnPosition == null) { respawnPosition = transform.position; }
    }

    // Update is called once per frame
    void Update()
    {
        DoSlowMotion();
        UpdateTimers();
        //currently it's a very limited scenario, but if we are in an animation, we want to take control away from the player
        BufferInput();
        if (!(climbing || dashing))
        {
            ProcessMovement();
        }
        RotateCamera(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //for debug
        if (Input.GetButtonDown("Interact"))
        {
            SnapSetRespawn();
        }
        if (Input.GetButtonDown("Respawn"))
        {
            Respawn();
        }

    }

    private void DoSlowMotion()
    {
        if (Input.GetButton("Fire1") && CurrentHealth > maxHealth * 0.1f && !slowing)
        {
            Time.timeScale = 0.25f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            if (OnSlowMotion != null)
            {
                OnSlowMotion(true);
            }
            slowing = true;
        }
        if (!(Input.GetButton("Fire1") && currentHealth > maxHealth * 0.1f) && slowing)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f;
            if (OnSlowMotion != null)
            {
                OnSlowMotion(false);
            }
            slowing = false;
        }
        if (slowing)
        {
            currentHealth -= (Time.deltaTime / Time.timeScale) * 10;
        }
    }
    private void BufferInput()
    {
        inputs.CleanStale();
        if (Input.GetButtonDown("Jump"))
        {
            inputs.EnqueueAction(InputAction.Jump);
        }
        if (Input.GetButtonDown("Dash"))
        {
            inputs.EnqueueAction(InputAction.Dash);
        }
    }
    private void UpdateTimers()
    {
        if (iFrameTimer > 0f)
        {
            iFrameTimer -= Time.deltaTime;
        }
        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
        }
        else if (currentDashes < maxDashes)
        {
            currentDashes++;
            dashTimer = DashCooldown;
            if (OnDashRestore != null)
            {
                OnDashRestore(CurrentDashes);
            }
        }
        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }
        if (cJumpTimer > 0f)
        {
            cJumpTimer -= Time.deltaTime;
            Debug.Log(cJumpTimer);
        }
    }

    private void ReceiveDamage(DamageInfo damage)
    {
        if (iFrameTimer <= 0f)
        {
            currentHealth -= damage.damage;
            iFrameTimer = iFrames;
            if (OnReceiveDamage != null)
            {
                OnReceiveDamage(damage);
            }
        }

        if (currentHealth <= 0)
        {
            Respawn();
            currentHealth = maxHealth;
        }
        else if (damage.forceRespawn)
        {
            Respawn();
            if (OnReceiveDamage != null)
            {
                OnReceiveDamage(new DamageInfo(-damage.damage, damage.forceRespawn));
            }
        }
    }

    private void Heal(float health)
    {
        if (health < 0)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth + health, 0, maxHealth);
        }
    }

    private void SnapSetRespawn()
    {
        respawnPosition = transform.position;
    }

    private void SetRespawn(Vector3 respawn_position)
    {
        respawnPosition = respawn_position;
    }

    private void Respawn()
    {
        transform.position = respawnPosition;
        rb.velocity = Vector3.zero;
        StartCoroutine(RespawnLock());
    }
    private IEnumerator RespawnLock()
    {
        var _timer = 0f;
        while (_timer < 0.1f)
        {
            _timer += Time.deltaTime;
            transform.position = respawnPosition;
            rb.velocity = Vector3.zero;
            yield return null;
        }

    }

    private void ProcessMovement()
    {
        if (inputs.CheckForAction(InputAction.Jump))
        {
            if ((OnWalkable() && jumpTimer <= 0f)||cJumpTimer>0f)
            {
                Jump();
                jumpTimer = jumpCooldown;
                cJumpTimer = 0f;
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

        if (inputs.CheckForAction(InputAction.Dash))
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
                if (OnCrouchChange != null)
                {
                    OnCrouchChange(crouching);
                }
            }
            //We want no player movement input during sliding
            col.height = 0.95f;
            col.center = new Vector3(0, 0.6f, 0);
            sph.center = new Vector3(0, 0.6f, 0);
            //transform.localScale = new Vector3(1, 0.5f, 1);
            cam.transform.localPosition = origCamPos * 0.5f;
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
                    if (OnCrouchChange != null)
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
        des_hor_vel *= maxSpeed;
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
        return Physics.SphereCastAll(
                    transform.TransformPoint(col.center) - new Vector3(0, 0.5f, 0),
                    0.5f,
                    Vector3.down,
                    0.1f
                ).Where(
                    x => x.transform.GetComponent<Walkable>() != null
                ).ToList().Count > 0;

    }

    private void Dash()
    {
        if (CurrentDashes > 0)
        {
            dashTimer = dashCooldown;
            currentDashes--;
            if (OnDash != null)
            {
                OnDash(currentDashes);
            }
            inputs.ClearAction(InputAction.Dash);
            StartCoroutine(DashLockout());
        }
    }
    private IEnumerator DashLockout()
    {
        dashing = true;
        for (float i = dashTimer; i > (dashCooldown - dashDuration); i = dashTimer)
        {
            var dash_dir = cam.transform.forward;
            dash_dir.y /= 20;
            dash_dir.Normalize();
            rb.velocity = dash_dir * dashForce;
            yield return null;
        }
        rb.velocity = rb.velocity.normalized * maxSpeed;
        dashing = false;
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        if (OnJump != null)
        {
            OnJump();
        }
        inputs.ClearAction(InputAction.Jump);
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
        inputs.ClearAction(InputAction.Jump);
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
        rb.velocity = (destination - midpoint).normalized * maxSpeed * 1.5f;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<Walkable>() != null && jumpTimer <= 0f)
        {
            List<ContactPoint> contacts = new();
            collision.GetContacts(contacts);
            if (contacts.Where(
                x => x.thisCollider.GetType() == typeof(SphereCollider)
                ).ToList().Count > 0)
            {
                Debug.Log("Set");
                cJumpTimer = coyoteJumpTime;
            };
        }

    }
}
public struct DamageInfo
{
    public float damage;
    public bool forceRespawn;

    public DamageInfo(float damage, bool forceRespawn)
    {
        this.damage = damage;
        this.forceRespawn = forceRespawn;
    }
}

public class ActionItem
{

    public enum InputAction { Jump, Dash };
    public InputAction Action;
    public float Timestamp;

    public static float TimeBeforeActionsExpire = 0.2f;

    //Constructor
    public ActionItem(InputAction ia, float stamp)
    {
        Action = ia;
        Timestamp = stamp;
    }

    //returns true if this action hasn't expired due to the timestamp
    public bool CheckIfValid()
    {
        bool returnValue = false;
        if (Timestamp + TimeBeforeActionsExpire >= Time.time)
        {
            returnValue = true;
        }
        return returnValue;
    }
}

public class InputBuffer : List<ActionItem>
{
    public void CleanStale()
    {
        while (Count > 0)
        {
            if (!this[0].CheckIfValid())
            {
                RemoveAt(0);
            }
            else
            {
                break;
            }
        }

    }

    public ActionItem PeekAction(InputAction inputAction)
    {
        for (int i = 0; i < Count; i++)
        {
            if (this.ElementAt(i).Action == inputAction)
            {
                return this.ElementAt(i);
            }
        }
        return null;
    }

    public ActionItem DequeueAction(InputAction inputAction)
    {
        for (int i = 0; i < Count; i++)
        {
            if (this.ElementAt(i).Action == inputAction)
            {
                var temp = this[i];
                RemoveAt(i);
                return temp;
            }
        }
        return null;
    }

    public void ClearAction(InputAction inputAction)
    {
        try
        {
            for (int i = 0; i < Count; i++)
            {
                if (this.ElementAt(i).Action == inputAction)
                {
                    RemoveAt(i);
                    i--;
                }
            }
        }
        catch
        {

        }
    }

    public bool CheckForAction(InputAction inputAction)
    {
        return PeekAction(inputAction) != null;
    }

    public void EnqueueAction(InputAction inputAction)
    {
        Add(new ActionItem(inputAction, Time.time));
    }
}