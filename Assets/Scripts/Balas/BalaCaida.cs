using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaCaida : MonoBehaviour
{
    //crear un objeto y volverlo prefab
    [Header("Explosion")]
    public float dmg;
    public float radioExplosion;
    [Header("Fisicas")]
    public Rigidbody rb; 
    public float speed = 20f;
    public float tiempoActivacion;
    public bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        rb.linearVelocity = transform.forward * speed; //cambiar transform.right en forward
        Invoke("Activar", tiempoActivacion);
        
    }
    void Activar()
    {
        active = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        if(!active) return;

        Explotar();
    }

    void Explotar()
    {
        Collider[]  objCercanos = Physics.OverlapSphere(transform.position, radioExplosion);
        foreach (Collider col in objCercanos)
        {
            EnemyManager enemigo = col.GetComponent<EnemyManager>();
            if(enemigo != null)
            {
                enemigo.RecibirDanio(dmg);
                MovimientoEnemigo mov = col.GetComponent<MovimientoEnemigo>();
                if(mov != null) mov.Alertar();

                MovimientoSniper sniper= col.GetComponent<MovimientoSniper>();
                if(sniper != null) sniper.Alertar();
            }
            PlayerManager ply = col.GetComponent<PlayerManager>();
            if(ply != null)
            {
                ply.RecibirDanio(dmg);
            }
        }
        Destroy(gameObject);
    }

}
