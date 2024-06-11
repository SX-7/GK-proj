using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : Hazard
{
    Camera target;
    [SerializeField] float speed=5;
    // Start is called before the first frame update
    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(target.transform.position-transform.position);
        transform.position += (target.transform.position - transform.position) * speed * Time.deltaTime;
    }
}
