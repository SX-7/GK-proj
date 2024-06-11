using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DashChargeManager : MonoBehaviour
{
    [SerializeField] GameObject dashCharge;
    [SerializeField] PlayerController playerController;
    [SerializeField] VisualsData visualsData;
    private float OrbitRadius { get => visualsData.orbitRadius; }
    private float OrbitSpeed { get => visualsData.orbitSpeed;}
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
        PlayerController.OnFire += OnFire;
    }

    private void OnDisable()
    {
        PlayerController.OnDashRestore -= OnDashRestore;
        PlayerController.OnDash -= OnDash;
        PlayerController.OnFire -= OnFire;
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

    private void OnFire(Vector3 target, float damage)
    {
        var sacrifice = dashCharges.Dequeue();
        sacrifice.GetComponent<DashCharge>().Kamikaze(target, damage);
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < dashCharges.Count; i++)
        {
            var dest = new Vector3(OrbitRadius * Mathf.Cos(i * 2 * Mathf.PI / dashCharges.Count), 0, OrbitRadius * Mathf.Sin(i * 2 * Mathf.PI / dashCharges.Count));
            dashCharges.ElementAt(i).transform.localPosition = Vector3.Lerp(dashCharges.ElementAt(i).transform.localPosition, dest, 0.05f);
        }
        
        transform.rotation *= Quaternion.AngleAxis(OrbitSpeed*Time.deltaTime, transform.up); //not intented, but definitely good looking result
    }
}
