using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Attributes")]
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    private Animator animator;

    public GameObject bulletPrefab;

    public Transform target;

    public Transform firePoint;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        
        fireCountdown -= Time.deltaTime;
        
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
