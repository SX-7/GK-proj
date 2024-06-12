using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    [Min(0f)]
    public float maxSpeed;
    [Min(0f)]
    public float jumpHeight;
    [Min(0f)]
    public float jumpCooldown;
    [Min(0f)]
    public float climbingTime;
    [Tooltip("After climbing, player is given speed burst in the direction of " +
        "ledge. Values above 1 let player exceed speed limits, especially when " +
        "in frictionless scenario (crouching)")]
    [Min(0f)]
    public float postClimbSpeedBurstFactor;

    [Header("Dashes")]
    [Min(0f)]
    public float dashCooldown;
    [Min(0f)]
    public float dashDuration;
    [Min(0f)]
    public float dashForce;
    [Min(0)]
    public int maxDashes;
    [Tooltip("Decreases amount of force given to vertical component of dash. " +
        "Vertical dash force will be divided by this number")]
    [Min(1f)]
    public float verticalDashForceDecrease;
    
    [Header("Health")]
    [Min(0f)]
    public float maxHealth;
    [Min(0f)]
    public float initialHealth;
    [Tooltip("In Seconds")]
    [Min(0f)]
    public float iFrames;

    [Header("Damage")]
    [Min(0f)]
    public float damage;
    [Tooltip("How far ahead the target of orb will be placed. " +
        "Since orbs fly from hand, and not from camera, too large values " +
        "will miss (no reticle convergence), " +
        "and too low will miss even more (flies to the side)")]
    [Min(0f)]
    public float targettingFocalPointDistance;

    [Header("Slow Motion")]
    [Tooltip("Multiplicative, ie. 2 equals 50% speed in slowdown")]
    [Min(1f)]
    public float slowDownFactor;
    [Tooltip("It's a percentage value")]
    [Range(0f,1f)]
    public float minimumHealthRequiredToSlowDown;
    [Tooltip("Should be x/second")]
    [Min(0f)]
    public float slowDownHealthDrain;

    [Header("Misc")]
    [Min(0f)]
    public float coyoteJumpTime;
    [Min(0f)]
    public float maxInteractDistance;
    [Min(0f)]
    public float respawnMovementLockoutTime;
    [Min(0f)]
    public float startingScore;
    [Min(0.1f)]
    public float scoreHalfTime;
}