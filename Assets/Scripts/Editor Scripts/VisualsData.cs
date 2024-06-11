using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/VisualsData", order = 1)]
public class VisualsData : ScriptableObject
{
    [Header("On Damage")]
    [Min(0f)]
    [Tooltip("In seconds")]
    public float onDamageRampupTime;
    [Min(0f)]
    [Tooltip("In seconds")]
    public float onDamageRampdownTime;
    public float onDamageVignetteStartScale;
    public float onDamageVignetteMaxScale;
    public float onDamageGrainStartScale;
    public float onDamageGrainMaxScale;

    [Header("On Dash")]
    [Min(0f)]
    public float onDashEffectDuration;
    public float onDashMaxEffectScale;

    [Header("Dash Orbs")]
    public float orbitRadius;
    [Tooltip("Degrees/Second")]
    public float orbitSpeed;

    [Header("Health")]
    [Tooltip("Degrees/Second")]
    public float vialRotationSpeed;
    [Tooltip("Left is 0%, right is 100%")]
    public Gradient healthColorGradient;
    [Range(0f, 1f)]
    public float healthLerpSpeed;

    [Header("Interact Hand")]
    [Tooltip("In seconds")]
    public float handMovementTime;
    [Tooltip("In seconds")]
    public float indicatorShrinkSpeed;
    [Range(0f, 1f)]
    [Tooltip("Percentage")]
    public float minimumSizeScale;

    [Header("Slow Mo")]
    [Range (0f, 1f)]
    public float slowMoChromaticEffectScale;
    public float slowMoEffectRampTime;

    [Header("Misc")]
    [Min(0f)]
    public float fadeTime;
}