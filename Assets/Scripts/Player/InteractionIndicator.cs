using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class InteractionIndicator : MonoBehaviour
{
    [SerializeField] VisualsData visualsData;
    private float ShrinkSpeed { get => visualsData.indicatorShrinkSpeed; }
    private float MinSizeScale { get => visualsData.minimumSizeScale; }
    private Vector3 startScale;
    private Quaternion startRotation;
    private void Start()
    {
        startScale = transform.localScale;
        startRotation = transform.localRotation;
    }
    private void OnEnable()
    {
        PlayerController.OnInteract += OnInteract;
        PlayerController.OnInteractPossibility += OnInteractPossibility;
    }

    private void OnDisable()
    {
        PlayerController.OnInteract -= OnInteract;
        PlayerController.OnInteractPossibility -= OnInteractPossibility;
    }

    private void OnInteractPossibility(bool state)
    {
        if (state)
        {
            transform.localRotation = startRotation;
        }
    }

    private void OnInteract()
    {
        transform.localScale = startScale;
        StopAllCoroutines();
        StartCoroutine(OnInteractCR());
    }
    IEnumerator OnInteractCR()
    {
        float timer = ShrinkSpeed;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            transform.localScale = startScale * Mathf.Lerp(1, MinSizeScale, (ShrinkSpeed - timer) / ShrinkSpeed);
            yield return null;
        }
        timer = ShrinkSpeed;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            transform.localScale = startScale * Mathf.Lerp(MinSizeScale, 1, (ShrinkSpeed - timer) / ShrinkSpeed);
            yield return null;
        }
    }

    void Update()
    {
        transform.SetPositionAndRotation(transform.position, Quaternion.Lerp(transform.rotation, Random.rotationUniform, Time.deltaTime));
    }
}
