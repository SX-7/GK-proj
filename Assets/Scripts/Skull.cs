using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skull : Hazard
{
    PlayerController player;
    Camera target;
    [SerializeField] float speed;
    private float speedSwap=0;
    // Start is called before the first frame update
    private void OnEnable()
    {
        PlayerController.OnRespawn += OnRespawn;
    }
    private void OnDisable()
    {
        PlayerController.OnRespawn -= OnRespawn;
    }
    private void OnRespawn(bool finishedLockout)
    {
        if (!finishedLockout) {
            
            speed = 0;
            transform.position = player.RespawnPosition + player.SkullRespawnOffset + target.transform.localPosition;
        }
        else
        {
            speed=speedSwap;
        }
    }
    private void Stop()
    {
        speed = 0;
        StartCoroutine(StopCR());
    }

    IEnumerator StopCR()
    {
        float timer = 0;
        while (timer < player.FadeTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        speed = speedSwap;   
    }

    private void PlaceAgain()
    {
        StartCoroutine(PlaceCR());
    }

    IEnumerator PlaceCR()
    {
        OnRespawn(false);
        float timer = 0;
        while(timer< player.FadeTime )
        {
            timer += Time.deltaTime;
            yield return null;
        }
        OnRespawn(true);
    }
    void Start()
    {
        speedSwap = speed;
        player = FindAnyObjectByType<PlayerController>();
        target = player.GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.position += (target.transform.position - transform.position) * speed * Time.deltaTime;
    }
}
