using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header(("Attributes"))]
    private Transform target;
    public float speed = 70f;
    public float lifetime = 5f;
    public float damageToPlayer = 0f;
    private float lifeTimer;
    private Vector3 direction;
    private PlayerController playerController;

    public GameObject impactEffect;
    public GameObject flyingEffect;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Start()
    {
        lifeTimer = lifetime;
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerController = playerObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Debug.Log("FIND PLAYER COMPONENT");
            }
            else
            {
                Debug.LogError("CANT FIND PLAYER COMPONENT");
            }
        }
        
        
    }
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        CountLifetime();
        MakeFlyingEffect();
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        transform.Translate(direction * distanceThisFrame, Space.World);

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }



    void CountLifetime()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
            return;
        }
        
    }
    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("HIT STH: " + collision.gameObject.name);
        MakeHitTargetEffect();
        TakeDmgToPlayer(collision);
        Destroy(gameObject);
    }

    private void TakeDmgToPlayer(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("ReceiveDamage", new DamageInfo(damageToPlayer, false), SendMessageOptions.DontRequireReceiver);
        }
    }

    void MakeFlyingEffect()
    {
        if (flyingEffect != null)
        {
            GameObject effectInstance = (GameObject)Instantiate(flyingEffect, transform.position, transform.rotation);
            Destroy(effectInstance, 2f);
        }
    }

    void MakeHitTargetEffect()
    {
        GameObject effectInstance = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 2f);
    }
}
