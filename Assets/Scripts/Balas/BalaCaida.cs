using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaCaida : MonoBehaviour
{
    //crear un objeto y volverlo prefab
    public float speed = 20f;
    public float dmg = 25f;
    public Rigidbody rb; //mismo codigo de 2d, solo que cambiar rigidbody 2d a rigidbody
    // Start is called before the first frame update
    void Start()
    {
        rb.linearVelocity = transform.forward * speed; //cambiar transform.right en forward
        Destroy(gameObject, 3f);
    }
    void OnTriggerEnter(Collider other)
    {
        EnemyManager salud = other.GetComponent<EnemyManager>();
        if (salud != null)
        {
            salud.RecibirDanio(dmg);
            Destroy(gameObject); 
        }

    }

    
     

}
