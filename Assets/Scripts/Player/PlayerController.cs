using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using static ActionItem;

public class PlayerController : MonoBehaviour
{
    //PlayerData relevant fields
    [SerializeField] PlayerData playerData;
    public float MaxSpeed { get => playerData.maxSpeed; }
    public float JumpHeight { get => playerData.jumpHeight; }
    public float JumpCooldown { get => playerData.jumpCooldown; }

    public float MaxHealth { get => playerData.maxHealth; }
    public float InitHealth { get => playerData.initialHealth; }
    public float IFrames { get => playerData.iFrames; }
    public float ClimbingTime { get => playerData.climbingTime; }
    public float DashCooldown { get => playerData.dashCooldown; }
    public float DashDuration { get => playerData.dashDuration; }
    public float DashForce { get => playerData.dashForce; }
    public int MaxDashes { get => playerData.maxDashes; }
    public float CoyoteJumpTime { get => playerData.coyoteJumpTime; }
    public float Damage { get => playerData.coyoteJumpTime; }
    public float MaxInteractDistance { get => playerData.maxInteractDistance; }
    public float SlowDownFactor { get => playerData.slowDownFactor; }
    public float TargettingFocalPointDistance { get => playerData.targettingFocalPointDistance; }
    public float MinimumHealthRequiredToSlowDown { get => playerData.minimumHealthRequiredToSlowDown; }
    public float SlowDownHealthDrain { get => playerData.slowDownHealthDrain; }
    public float RespawnMovementLockoutTime { get => playerData.respawnMovementLockoutTime; }
    public float VerticalDashForceDecrease { get => playerData.verticalDashForceDecrease; }
    public float PostClimbSpeedBurstFactor { get => playerData.postClimbSpeedBurstFactor; }
    [SerializeField] VisualsData visualData;
    public float FadeTime { get => visualData.fadeTime; }
    //Timers
    private float jumpTimer = 0f;
    private float iFrameTimer = 0f;
    private float dashTimer = 0f;
    private float dashRestoreTimer = 0f;
    private float cJumpTimer = 0f;
    //Internals
    private float currentHealth;
    public float CurrentHealth { get => currentHealth; }
    private float viewYAngle = 0f;
    private float viewXAngle = 0f;
    private List<Collider> vaultTargets = new();
    private Vector3 origCamPos;
    private int currentDashes = 0;
    public int CurrentDashes { get => currentDashes; }
    public Vector3 Velocity { get => rb.velocity; }
    private InputBuffer inputs = new();
    private List<Interactable> interactables = new();
    private float capsuleColliderInitHeight;
    private Vector3 capsuleColliderInitCenter;
    private Vector3 sphereColliderInitCenter;
    public static int Score;
    //State trackers
    private bool crouching = false;
    private bool dashing = false;
    private bool slowing = false;
    private bool isPaused = false;
    private bool climbing = false;
    private bool completeStasis = false;
    private bool dead = false;

    //Delegates/Events
    public delegate void ReceiveDamageAction(DamageInfo damageInfo);
    public static event ReceiveDamageAction OnReceiveDamage;
    public delegate void CrouchAction(bool crouching);
    public static event CrouchAction OnCrouchChange;
    public delegate void JumpAction();
    public static event JumpAction OnJump;
    public delegate void DashAction(int currentDashes);
    public static event DashAction OnDash;
    public delegate void DashRestoreAction(int currentDashes);
    public static event DashRestoreAction OnDashRestore;
    public delegate void SlowMotionStateChange(bool enabled);
    public static event SlowMotionStateChange OnSlowMotion;
    public delegate void FireAction(Vector3 target, float damage);
    public static event FireAction OnFire;
    public delegate void InteractPossibility(bool interactableInRange);
    public static event InteractPossibility OnInteractPossibility;
    public delegate void Interact();
    public static event Interact OnInteract;
    public delegate void PauseAction(bool paused);
    public static event PauseAction OnPause;
    //External references
    [Header("Object References")]
    [Tooltip("Rigidbody")][SerializeField] Rigidbody rb;
    [Tooltip("Camera")][SerializeField] Camera cam;
    [Tooltip("Capsule collider, fullbody one")][SerializeField] CapsuleCollider col;
    [Tooltip("Sphere collider, for the feet")][SerializeField] SphereCollider sph;
    [Tooltip("Pause menu object")][SerializeField] GameObject pauseMenu;
    [Tooltip("Fade in/out ppv")][SerializeField] PostProcessVolume ppv;
    [Header("Context Relevant Variables")]
    [Tooltip("Respawn position")][SerializeField] Vector3 respawnPosition;
    [Header("Physics/Rigidbody Variables - Preferably don't edit")]
    [Tooltip("Percentage Value")][SerializeField] float crouchColliderScale = 0.5f;
    [SerializeField] Vector3 capsuleColliderCrouchCenter = new(0, 0.6f, 0);
    [SerializeField] Vector3 sphereColliderCrouchCenter = new(0, 0.6f, 0);
    [SerializeField] Vector3 cameraCrouchPosition = new(0, 0.9f, 0);
    [SerializeField] float maxVerticalCameraAngle = 80;
    [SerializeField] float minVerticalCameraAngle = -80;
    private void Awake()
    {
        //for some reason debug stuff gets enabled, and idk where to disable it permanenetly
        //so, bandaid fix
        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
    }
    void Start()
    {
        cam.fieldOfView = DataStore.Instance.FOV;
        ppv.weight = 1;
        //make sure we initialize with something
        if (rb == null) { rb = GetComponent<Rigidbody>(); }
        if (cam == null) { cam = GetComponentInChildren<Camera>(); }
        col = col != null ? col : GetComponent<CapsuleCollider>();
        capsuleColliderInitHeight = col.height;
        capsuleColliderInitCenter = col.center;
        if (sph == null) { sph = GetComponent<SphereCollider>(); }
        sphereColliderInitCenter = sph.center;
        origCamPos = cam.transform.localPosition;
        if (respawnPosition == null) { respawnPosition = transform.position; }
        currentHealth = InitHealth;
        Cursor.visible = isPaused;
        SnapSetRespawn();
        FadeIn();
    }



    // Update is called once per frame
    void Update()
    {
        //Process pause
        if (Input.GetButtonDown("Cancel"))
        {
            if (isPaused)
            {
                if (!slowing)
                {
                    Time.timeScale = 1.0f;
                }
                else
                {
                    Time.timeScale = 1.0f / SlowDownFactor;
                }
                //smoothening magic :shrug:
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                isPaused = false;
            }
            else
            {
                Time.timeScale = 0f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                isPaused = true;
            }
            OnPause?.Invoke(isPaused);
            pauseMenu.SetActive(isPaused);
            Cursor.visible = isPaused;
        }
        if (!isPaused & !dead)
        {
            DoSlowMotion();
            UpdateTimers();
            BufferInput();
            //currently it's a very limited scenario, but if we are in an animation, we want to take control away from the player
            if (!(climbing || dashing))
            {
                ProcessActions();
            }
            RotateCamera(Input.GetAxis("Mouse X")*DataStore.Instance.Sensitivity, Input.GetAxis("Mouse Y")*DataStore.Instance.Sensitivity);
            UpdateInteractables();
            if (Input.GetButtonDown("Interact"))
            {
                AttemptInteract();
            }
            //if (Input.GetButtonDown("Respawn"))
            //{
            //    Respawn();
            //}

        }

    }

    private void UpdateInteractables()
    {
        var prevState = false;
        if (interactables.Any())
        {
            prevState = true;
        }
        interactables = Physics.RaycastAll(
            cam.transform.position,
            cam.transform.forward,
            MaxInteractDistance
            ).Where(
            x => x.transform.GetComponent<Interactable>() != null
            ).Select(x => x.transform.GetComponent<Interactable>()).ToList();
        if (interactables.Any() && !prevState && OnInteractPossibility != null)
        {
            OnInteractPossibility(true);
        }
        if (!interactables.Any() && prevState)
        {
            OnInteractPossibility(false);
        }
    }

    private void AttemptInteract()
    {
        if (interactables.Any())
        {
            interactables.First().SendMessage("Interact");
            OnInteract?.Invoke();
        }
    }

    //for menu
    private void UnPause()
    {
        if (isPaused)
        {
            if (!slowing)
            {
                Time.timeScale = 1.0f;
            }
            else
            {
                Time.timeScale = 1.0f / SlowDownFactor;
            }
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            isPaused = false;
        }
        OnPause?.Invoke(isPaused);
        pauseMenu.SetActive(isPaused);
        Cursor.visible = isPaused;
    }
    private void DoSlowMotion()
    {
        if (Input.GetButton("Fire1") && CurrentHealth > MaxHealth * MinimumHealthRequiredToSlowDown && !slowing)
        {
            Time.timeScale = 1.0f / SlowDownFactor;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            OnSlowMotion?.Invoke(true);
            slowing = true;
        }
        if (!(Input.GetButton("Fire1") && currentHealth > MaxHealth * MinimumHealthRequiredToSlowDown) && slowing)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f;
            OnSlowMotion?.Invoke(false);
            slowing = false;
        }
        if (slowing)
        {
            currentHealth -= (Time.deltaTime / Time.timeScale) * SlowDownHealthDrain;
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
        if (Input.GetButtonDown("Fire2"))
        {
            inputs.EnqueueAction(InputAction.Fire);
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
        if (dashRestoreTimer > 0f)
        {
            dashRestoreTimer -= Time.deltaTime;
        }
        else if (currentDashes < MaxDashes)
        {
            currentDashes++;
            dashRestoreTimer = DashCooldown;
            OnDashRestore?.Invoke(CurrentDashes);
        }
        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }
        if (cJumpTimer > 0f)
        {
            cJumpTimer -= Time.deltaTime;
        }
    }

    private void ReceiveDamage(DamageInfo damage)
    {
        if (iFrameTimer <= 0f)
        {
            currentHealth -= damage.damage;
            iFrameTimer = IFrames;
            OnReceiveDamage?.Invoke(damage);
        }

        if (currentHealth <= 0)
        {
            Death();
        }
        else if (damage.forceRespawn)
        {
            Respawn();
            OnReceiveDamage?.Invoke(new DamageInfo(-damage.damage, damage.forceRespawn));
        }
    }

    private void Heal(float health)
    {
        if (health < 0)
        {
            //ez full heal
            currentHealth = MaxHealth;
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth + health, 0, MaxHealth);
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
        //stops dashes
        dashTimer = 0f;
        StartCoroutine(RespawnLock());
    }
    private IEnumerator RespawnLock()
    {
        var _timer = 0f;
        while (_timer < RespawnMovementLockoutTime)
        {
            _timer += Time.deltaTime;
            transform.position = respawnPosition;
            rb.velocity = Vector3.zero;
            yield return null;
        }

    }

    private void ProcessActions()
    {
        if (inputs.CheckForAction(InputAction.Jump))
        {
            if ((OnWalkable() && jumpTimer <= 0f) || cJumpTimer > 0f)
            {
                Jump();
                jumpTimer = JumpCooldown;
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
            //it is possible to stop crouch when you can't physically uncrouch,
            //due to no limited friction during crouch, we allow movement to avoid softlocks
            CrouchingMode(false);
            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        if (inputs.CheckForAction(InputAction.Dash))
        {
            Dash();
        }

        if (inputs.CheckForAction(InputAction.Fire) && currentDashes > 0)
        {
            if (currentDashes == MaxDashes)
            {
                dashRestoreTimer = DashCooldown;
            }
            currentDashes -= 1;
            inputs.ClearAction(InputAction.Fire);
            if (OnFire != null)
            {
                var result = cam.transform.position + cam.transform.forward * TargettingFocalPointDistance;
                OnFire(result, Damage);
            }
        }
    }

    private void CrouchingMode(bool crouch)
    {
        if (crouch)
        {
            if (!crouching)
            {
                crouching = true;
                OnCrouchChange?.Invoke(crouching);
            }
            //We want no player movement input during sliding
            col.height = capsuleColliderInitHeight * crouchColliderScale;
            //col.center = new Vector3(0, 0.6f, 0);
            //sph.center = new Vector3(0, 0.6f, 0);
            col.center = capsuleColliderCrouchCenter;
            sph.center = sphereColliderCrouchCenter;
            //transform.localScale = new Vector3(1, 0.5f, 1);
            //cam.transform.localPosition = origCamPos * 0.5f;
            cam.transform.localPosition = cameraCrouchPosition;
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
                    OnCrouchChange?.Invoke(crouching);
                }
                col.height = capsuleColliderInitHeight;
                col.center = capsuleColliderInitCenter;
                sph.center = sphereColliderInitCenter;
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
        des_hor_vel *= MaxSpeed;
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
            dashRestoreTimer = DashCooldown;
            currentDashes--;
            OnDash?.Invoke(currentDashes);
            inputs.ClearAction(InputAction.Dash);
            StartCoroutine(DashLockout());
        }
    }
    private IEnumerator DashLockout()
    {
        dashing = true;
        dashTimer = DashDuration;
        while (dashTimer > 0)
        {
            var dash_dir = cam.transform.forward;
            dash_dir.y /= VerticalDashForceDecrease;
            dash_dir.Normalize();
            rb.velocity = dash_dir * DashForce;
            yield return null;
        }
        rb.velocity = rb.velocity.normalized * MaxSpeed;
        dashing = false;
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * JumpHeight, ForceMode.Impulse);
        OnJump?.Invoke();
        inputs.ClearAction(InputAction.Jump);
    }

    private void RotateCamera(float x_delta, float y_delta)
    {
        //helper function
        viewYAngle -= y_delta;
        viewYAngle = Mathf.Clamp(viewYAngle, minVerticalCameraAngle, maxVerticalCameraAngle);
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
        var anim_time = ClimbingTime;
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
        //1.5f
        rb.velocity = PostClimbSpeedBurstFactor * MaxSpeed * (destination - midpoint).normalized;
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
                cJumpTimer = CoyoteJumpTime;
            };
        }

    }

    private void Exit()
    {
        UnPause();
        StartCoroutine(ExitCR(0f));
    }

    private void GracefulExit()
    {
        UnPause();
        LockMovement(1.5f);
        StartCoroutine(FadeOutCR(1f));
        StartCoroutine(ExitCR(1.1f));
        completeStasis = true;
        dead = true;
    }
    IEnumerator ExitCR(float min_time)
    {
        var timer = 0f;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenu");
        while (!asyncLoad.isDone || timer<min_time)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));
    }

    private void FadeIn()
    {
        StopCoroutine(FadeInCR(FadeTime));
        StopCoroutine(FadeOutCR(FadeTime));
        ppv.weight = 1;
        StartCoroutine(FadeInCR(FadeTime));
    }

    IEnumerator FadeInCR(float fade_time)
    {
        float timer = 0f;
        while (timer < fade_time)
        {
            timer += Time.deltaTime;
            ppv.weight = Mathf.Lerp(1, 0, timer / fade_time);
            yield return null;
        }
        ppv.weight = 0;
    }

    private void FadeOut()
    {
        StopCoroutine(FadeInCR(FadeTime));
        StopCoroutine(FadeOutCR(FadeTime));
        ppv.weight = 0;
        StartCoroutine(FadeOutCR(FadeTime));
    }

    IEnumerator FadeOutCR(float fade_time)
    {
        float timer = 0f;
        while (timer < fade_time)
        {
            timer += Time.deltaTime;
            ppv.weight = Mathf.Lerp(0, 1, timer / fade_time);
            yield return null;
        }
        ppv.weight = 1;
    }

    private void LockMovement(float duration)
    {
        StopCoroutine(LockMovementCR(duration));
        StartCoroutine(LockMovementCR(duration));
    }

    public IEnumerator LockMovementCR(float duration)
    {
        var timer = 0f;
        var pin = transform.position;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.position = pin;
            rb.velocity = Vector3.zero;
            yield return null;
        }
    }

    private void Death()
    {
        if (!completeStasis)
        {
            FadeOut();
            LockMovement(FadeTime);
            completeStasis = true;
            StartCoroutine(DeathCR());
        }
    }

    IEnumerator DeathCR()
    {
        var timer = 0f;
        while(timer< FadeTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(DeathSceneCR());
    }

    //TODO: no magic variable here
    IEnumerator DeathSceneCR()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Death Scene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Death Scene"));
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

    public enum InputAction { Jump, Dash, Fire };
    public InputAction Action;
    public float Timestamp;
    //currently hardcoded cuz it's a seperate class
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