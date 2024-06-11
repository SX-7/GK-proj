using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    public float speed = 70f;
    public float lifetime = 5f;
    private float lifeTimer;
    private Vector3 direction;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Start()
    {
        lifeTimer = lifetime;
    }
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        CountLifetime();

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        transform.Translate(direction * distanceThisFrame, Space.World);

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }
        
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        Debug.Log("HIT STH");
        Destroy(gameObject);
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

        Destroy(gameObject);
    }
}
