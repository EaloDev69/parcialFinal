using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using  System;

public class Prefab : MonoBehaviour
{
    public static event Action<int, int, int> OnAmmoChanged;
    //este codigo va en player/jugador o como lo llamen
    [Header("Disparo")]
    public Transform firePoint; //Usen la camara por ahora
    public GameObject bala; //Prefab de bala

    [Header("Municiones")]
    public int Cargador; 
    public int MaxMag; //Modificar segun el arma antes de play
    public int CargadoresTotales; //Modificar segun el arma antes de play

    [Header("Cadencia")]
    public float fireRate; // Segundos entre disparo y disparo
    private float nextFireTime; // Cuándo puede volver a disparar

    void Start()
    {
        //Primer cargador
        Cargador = MaxMag;
        NotificarHUD();
    }

    // Update is called once per frame
    void Update()
    {
        //Disparo y  limitar canntidad  de balas
        if (Input.GetButton("Fire1") && Cargador > 0 && Time.time >= nextFireTime)
        {
            Disparar();
        }
        //Recargar y limitar cargadores
        if (Input.GetKeyDown(KeyCode.R) && CargadoresTotales > 0)
        {
            Recargar();
        }
        
    }

    public void Disparar()
    {
        //Disparo en si
        Instantiate(bala, firePoint.position, firePoint.rotation);
        Cargador--; //Vaciar cargas
        nextFireTime = Time.time + fireRate; //rafagas
        NotificarHUD();
    }
    public void Recargar()
    {
        if(Cargador < MaxMag){
        Cargador = MaxMag; //Recargamos
        CargadoresTotales--; //1 cargador menos
        NotificarHUD();
        }
    }
    void NotificarHUD()
    {
        OnAmmoChanged?.Invoke(Cargador, MaxMag, CargadoresTotales);
    }
}
