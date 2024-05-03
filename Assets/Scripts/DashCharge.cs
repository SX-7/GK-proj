using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashCharge : MonoBehaviour
{
    [SerializeField] ParticleSystem pSys;
    [SerializeField] ParticleSystem pCharge;
    [SerializeField] MeshRenderer mRer;
    [SerializeField] Light lig;
    private float timer = 0f;
    public void DeathBurst()
    {
        //burst into tons of glowy particles!
        timer = -10f;
        pSys.Emit(25);
        lig.enabled = false;
        mRer.enabled = false;
        var main = pSys.emission;
        main.enabled = false;
    }


    private void Start()
    {
        if (pSys == null) { throw new UnassignedReferenceException("Particle system not assigned"); }
        if (pCharge == null) { throw new UnassignedReferenceException("Particle system not assigned"); }
        if (mRer == null) { mRer = GetComponent<MeshRenderer>(); }
        if (lig == null) { lig = GetComponent<Light>(); }
        lig.enabled = false;
        mRer.enabled = false;
        var main = pSys.emission;
        main.enabled = false;
        pCharge.Play();
    }
    private void Update()
    {
        if (timer > -1f)
        {
            if (timer < 0.2f)
            {
                timer += Time.deltaTime;
            }
            else
            {
                mRer.enabled = true;
                lig.enabled = true;
                var main = pSys.emission;
                main.enabled = true;
                pSys.Emit(50);
                timer = -10f;
            }
        }
    }
}
