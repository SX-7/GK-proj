using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DashCharge : MonoBehaviour
{
    [SerializeField] ParticleSystem pSys;
    [SerializeField] ParticleSystem pCharge;
    [SerializeField] ParticleSystem pExplode;
    [SerializeField] MeshRenderer mRer;
    [SerializeField] Light lig;
    [SerializeField] Collider col;
    [SerializeField] TrailRenderer tra;
    private float timer = 0f;
    private float damage = 50f;
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

    public void Kamikaze(Vector3 target, float _damage)
    {
        lig.range *= 5;
        lig.intensity *= 5;
        col.enabled = true;
        tra.enabled = true;
        damage = _damage;
        StartCoroutine(SendIt(transform.position, target, 20f));
    }

    private IEnumerator SendIt(Vector3 start, Vector3 end, float vel)
    {
        var dist = Vector3.Distance(start, end);
        var ratio = dist / vel;
        var timed = 0f;
        while (timed < ratio)
        {
            timed += Time.deltaTime;
            timed = Mathf.Clamp(timed, 0f, ratio);
            transform.position = Vector3.Lerp(start, end, timed / ratio);
            yield return null;
        }
        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.GetComponent<Walkable>() != null || other.gameObject.GetComponent<Collider>().isTrigger == false) && other.gameObject.GetComponent<PlayerController>() == null)
        {
            Explode();
        }
    }

    private void Explode()
    {
        StopAllCoroutines();
        pExplode.Emit(100);
        var main = pSys.emission;
        main.enabled = false;
        mRer.enabled = false;
        tra.enabled = false;
        lig.enabled = false;
        col.enabled = false;
        var to_destroy = Physics.OverlapSphere(transform.position, 2f).Where((x) => x.gameObject.GetComponent<Destructible>() != null).ToList();
        foreach (var t in to_destroy) t.SendMessage("ReceiveDamage", damage);
        //player knockback, cuz funi :)
        var knockback = Physics.OverlapSphere(transform.position, 2f).Where((x) => x.gameObject.GetComponent<PlayerController>() != null).ToList();
        if (knockback.Count > 0)
        {
            var knockback_dir = knockback[0].GetComponent<Rigidbody>().centerOfMass +
                knockback[0].GetComponent<PlayerController>().transform.position -
                transform.position;
            var knockback_strength = 10 / knockback_dir.magnitude;
            knockback[0].GetComponent<Rigidbody>().AddForce(knockback_dir.normalized*knockback_strength, ForceMode.Impulse);
        }
        Destroy(gameObject, 1);
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
