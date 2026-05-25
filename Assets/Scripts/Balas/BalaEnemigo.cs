using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaEnemigo : MonoBehaviour
{
    //crear un objeto y volverlo prefab
    public float speed = 20f;
    public float dmg = 25f;
    public Rigidbody rb; 
    void Start()
    {
        rb.linearVelocity = transform.forward * speed; //cambiar transform.right en forward
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerManager salud = other.GetComponent<PlayerManager>();
        if (salud != null)
        {
            salud.RecibirDanio(dmg);
            Destroy(gameObject); 
        }

    }
    
     

}
