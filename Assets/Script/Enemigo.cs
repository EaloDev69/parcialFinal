using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [Header("Salud")]
    public float salud;

    [Header("Disparo")]
    public Transform firePoint;
    public GameObject bala;

    private GameObject target;
    private Animator anim;

    [SerializeField] private float tiempoDisparo;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float distancia = Vector2.Distance(transform.position, target.transform.position);        

        if(distancia < 8)
        {
            tiempoDisparo += Time.deltaTime;

            if (tiempoDisparo > 2)
            {
                tiempoDisparo = 0;
                Disparar();
            }
        }
    }

    private void Disparar()
    {
        Instantiate(bala, firePoint.position, firePoint.rotation);
    }

    public void getDamage(float dmg)
    {
        salud -= dmg;

        if (salud <= 0)
        {
            anim.SetTrigger("Dead");
        }
    }

    public void Dead()
    {
        Destroy(gameObject);
    }
}
