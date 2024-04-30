using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbable : MonoBehaviour
{
    public List<float> offsets = new List<float>();
    [SerializeField] BoxCollider bc;

    void OnDrawGizmosSelected()
    {
        if (bc == null) { bc = GetComponent<BoxCollider>(); }
        Gizmos.color = Color.yellow;
        foreach (var entry in offsets)
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(0, entry, 0), new Vector3(bc.transform.localScale.x, 0, bc.transform.localScale.z));
        }

    }
}
