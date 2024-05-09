using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] Light lig;
    [SerializeField] Color defaultLightColor;
    [SerializeField] float defaultLightAngle = 30;
    [SerializeField] float defaultLightIntensity = 1;
    [SerializeField] float defaultLightRange = 5;
    [SerializeField] Color crouchingLightColor;
    [SerializeField] float crouchLightAngle = 60;
    [SerializeField] float crouchLightIntensity = 0.9f;
    [SerializeField] float crouchLightRange = 2.5f;
    [SerializeField] ParticleSystem pSys;
    [SerializeField] ParticleSystem.MinMaxGradient defaultGradient;
    [SerializeField] float defaultParticleAngle = 45;
    [SerializeField] float defaultParticleSpeed = 40;
    [SerializeField] ParticleSystem.MinMaxGradient crouchGradient;
    [SerializeField] float crouchParticleAngle = 75;
    [SerializeField] float crouchParticleSpeed = 20;
    [SerializeField] int particlesEmittedOnJump = 25;
    // Start is called before the first frame update
    void Start()
    {
        lig = lig != null ? lig : GetComponent<Light>();
        pSys = pSys != null ? pSys : GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        PlayerController.OnCrouchChange += OnCrouchChange;
        PlayerController.OnJump += OnJump;
    }

    private void OnDisable()
    {
        PlayerController.OnCrouchChange -= OnCrouchChange;
        PlayerController.OnJump -= OnJump;
    }

    private void OnCrouchChange(bool crouching)
    {
        pSys.Clear();
        var cot = pSys.colorOverLifetime;
        var shp = pSys.shape;
        var main = pSys.main;
        if (crouching)
        {
            transform.localPosition -= new Vector3(0, 0.75f, 0);
            cot.color = crouchGradient;
            shp.angle = crouchParticleAngle;
            main.startSpeed = crouchParticleSpeed;
            lig.intensity = crouchLightIntensity;
            lig.range = crouchLightRange;
            lig.spotAngle = crouchLightAngle;
            lig.innerSpotAngle = crouchLightAngle/2;
            lig.color = crouchingLightColor;
        }
        else
        {
            transform.localPosition += new Vector3(0, 0.75f, 0);
            cot.color = defaultGradient;
            shp.angle = defaultParticleAngle;
            main.startSpeed = defaultParticleSpeed;
            lig.intensity = defaultLightIntensity;
            lig.range = defaultLightRange;
            lig.spotAngle = defaultLightAngle;
            lig.innerSpotAngle = defaultLightAngle / 2;
            lig.color = defaultLightColor;
        }
    }

    private void OnJump()
    {
        pSys.Emit(particlesEmittedOnJump);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
