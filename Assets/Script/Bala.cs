using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody rb;
    public float damage;
    Vector3 velocidad;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }

    private void Update()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.name);

        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerManager>().getDamage(damage);
            Destroy(gameObject);
        }

        if (collision.CompareTag("Enemigo"))
        {
            collision.GetComponent<Enemigo>().getDamage(damage);
            Destroy(gameObject);
        }
    }
}
