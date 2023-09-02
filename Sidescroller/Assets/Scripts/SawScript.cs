using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawScript : MonoBehaviour
{
   public float speed = 300;

    void Update()
    {
        transform.Rotate(Vector3.forward * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider c)
    {
        if(c.tag == "Player")
        {
            Debug.Log("Ouchy");
            c.GetComponent<Entity>().TakeDamage(5);
        }
    }
}
