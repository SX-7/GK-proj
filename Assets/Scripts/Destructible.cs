using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    //If you need to create enemies: Extend this class, and overwrite methods
    //Remember you can also have multiple script components - all you might need is just slapping it onto a thing you want destructible, and calling it a day!
    [SerializeField] Collider col;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float currentHealth = 100;
    void Start()
    {
        if (GetComponent<Collider>() == null) { throw new MissingComponentException("Missing collider"); }
        if (col == null) { col = GetComponent<Collider>(); }
    }

    void ReceiveDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Got hit for " + damage + " curr: " + currentHealth);
        if (currentHealth <= 0)
        {
            NoHealth();
        }
    }

    void Heal(float healing)
    {
        currentHealth += healing;
        // I think it's funnier to just accept negative healing
        if (currentHealth <= 0)
        {
            NoHealth();
        }
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    }
    void NoHealth()
    {
        Destroy(gameObject);
    }
}
