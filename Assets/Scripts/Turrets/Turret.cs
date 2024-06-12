using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

    
    
    [Header("Use Bullets")]
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    [Header("Use Laser")] 
    public bool useLaser = false;

    public LineRenderer lineRenderer;
    
    [Header("Unity Setup")]

    public GameObject bulletPrefab;

    public Transform target;

    public Transform firePoint;
    void Start()
    {

    }
    
    void Update()
    {
        if (useLaser)
        {
            Laser();
        }

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
    
        fireCountdown -= Time.deltaTime;
        
    }

    void Laser()
    {
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);
    }

    

    void Shoot()
    {
        Debug.Log("Shoot");
        GameObject bulletGameObject = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGameObject.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(target);
        }
    }
}
