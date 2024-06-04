using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHand : MonoBehaviour
{
    [SerializeField] float downDistance = 1.0f;
    [SerializeField] float movementTime = 1.0f;
    private Vector3 initPos;
    private Vector3 downPos;
    private Vector3 desPos;
    private void Start()
    {
        initPos = transform.localPosition + Vector3.zero;
        transform.localPosition = transform.localPosition + new Vector3(0,-downDistance,0);
        downPos = transform.localPosition + Vector3.zero;
        desPos = downPos;
    }

    private void OnEnable()
    {
        PlayerController.OnInteractPossibility += OnInteractPossibility;
    }

    private void OnDisable()
    {
        PlayerController.OnInteractPossibility -= OnInteractPossibility;
    }

    private void OnInteractPossibility(bool interactableInRange)
    {
        if (interactableInRange)
        {
            desPos = initPos;
        }
        else
        {
            desPos = downPos;
        }
        StopAllCoroutines();
        StartCoroutine(MoveToCR(desPos));
    }

    IEnumerator MoveToCR(Vector3 destination)
    {
        var startPos = transform.localPosition;
        var timer = movementTime;
        while (timer > 0)
        {
            timer-= Time.deltaTime;
            transform.localPosition = Vector3.Slerp(startPos, destination, (movementTime-timer)/movementTime);
            yield return null;
        }
    }

}
