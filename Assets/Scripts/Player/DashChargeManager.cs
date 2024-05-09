using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class DashChargeManager : MonoBehaviour
{
    [SerializeField] GameObject dashCharge;
    [SerializeField] PlayerController playerController;
    [SerializeField] float orbitRadius = 0.5f;
    [SerializeField] float orbitSpeed = 30f;
    private Queue<GameObject> dashCharges = new Queue<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        if (dashCharge == null) { throw new UnassignedReferenceException("Dash Charge prefab was not slotted"); }
        if (playerController == null) { throw new UnassignedReferenceException("Player was not connected"); }
    }
    private void OnEnable()
    {
        PlayerController.OnDashRestore += OnDashRestore;
        PlayerController.OnDash += OnDash;
    }

    private void OnDisable()
    {
        PlayerController.OnDashRestore -= OnDashRestore;
        PlayerController.OnDash -= OnDash;
    }

    private void OnDash(int dashCount)
    {
        var target = dashCharges.Dequeue();
        target.BroadcastMessage("DeathBurst");
        Destroy(target,1f);
    }

    private void OnDashRestore(int dashCount)
    {
        dashCharges.Enqueue(Instantiate(dashCharge, transform));
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < dashCharges.Count; i++)
        {
            var dest = new Vector3(orbitRadius * Mathf.Cos(i * 2 * Mathf.PI / dashCharges.Count), 0, orbitRadius * Mathf.Sin(i * 2 * Mathf.PI / dashCharges.Count));
            dashCharges.ElementAt(i).transform.localPosition = Vector3.Lerp(dashCharges.ElementAt(i).transform.localPosition, dest, 0.05f);
        }
        
        transform.rotation *= Quaternion.AngleAxis(orbitSpeed*Time.deltaTime, transform.up); //not intented, but definitely good looking result
    }
}
