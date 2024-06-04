using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionIndicator : MonoBehaviour
{
    [SerializeField] float shrinkSpeed = 0.2f;
    [SerializeField] float minSizeScale = 0.8f;
    private Vector3 startScale;
    private void Start()
    {
        startScale = transform.localScale;
    }
    private void OnEnable()
    {
        PlayerController.OnInteract += OnInteract;
    }

    private void OnDisable()
    {
        PlayerController.OnInteract -= OnInteract;
    }

    private void OnInteract()
    {
        transform.localScale = startScale;
        StopAllCoroutines();
        StartCoroutine(OnInteractCR());
    }
    IEnumerator OnInteractCR()
    {
        float timer = shrinkSpeed;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            transform.localScale = startScale * Mathf.Lerp(1, minSizeScale, (shrinkSpeed - timer) / shrinkSpeed);
            yield return null;
        }
        timer = shrinkSpeed;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            transform.localScale = startScale * Mathf.Lerp(minSizeScale, 1, (shrinkSpeed - timer) / shrinkSpeed);
            yield return null;
        }
    }

    void Update()
    {
        transform.SetPositionAndRotation(transform.position, Quaternion.Lerp(transform.rotation, Random.rotationUniform, Time.deltaTime));
    }
}
