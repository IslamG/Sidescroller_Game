using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float health;
    public GameObject ragdoll;

    public void TakeDamage(float dmg)
    {
        health -= dmg;

        if(health <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Ded");
        Ragdoll r = (Instantiate(ragdoll, transform.position, transform.rotation) as GameObject).GetComponent<Ragdoll>();
        Destroy(this.gameObject);
    }
}
