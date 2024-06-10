using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] Collider hurtArea;
    [SerializeField] float damage;
    [SerializeField] bool forcesRespawn;
    // Start is called before the first frame update
    void Start()
    {
        if(hurtArea == null) hurtArea = GetComponent<Collider>();
        if(!hurtArea.isTrigger) throw new UnassignedReferenceException("No trigger area found/assigned on object "+ name);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            other.gameObject.BroadcastMessage("ReceiveDamage", new DamageInfo(damage, forcesRespawn));
        };
    }
}
