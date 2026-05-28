using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using  System;

public class Escopeta : MonoBehaviour
{
    public static event Action<int, int, int, bool> OnAmmoChanged;
    [Header("Configuración de Disparo")]
    public GameObject balaPrefab;         
    public Transform puntoDisparo;        
    public float cadencia = 1f;           

    [Header("Configuración de Perdigones")]
    public int cantidadPerdigones = 8;    
    public float anguloDispersion = 15f;  
    private float tiempoUltimoDisparo = 0f;

    [Header("Municiones")]
    public int Cargador; 
    public int MaxMag;
    public int CargadoresTotales;
    public float recargaTime;
    private float recargaContador;
    public bool recargando = false;
    void OnEnable()
    {
        NotificarHUD();
    }
    void Start()
    {
        //Primer cargador
        Cargador = MaxMag; 
        recargaContador = recargaTime; // corregido desde el principio
        NotificarHUD();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && PuedoDisparar() && Cargador > 0 && !recargando)
        {
            Disparar();
            tiempoUltimoDisparo = Time.time + cadencia;
        }
        if (Input.GetKeyDown(KeyCode.R) && CargadoresTotales > 0)
        {
            Recargar();
        }
            if (recargando)
            {
                recargaContador -= Time.deltaTime;
                if (recargaContador <= 0)
                {
                    recargando = false;
                    recargaContador = recargaTime;
                    NotificarHUD();
                }
            }
        
    }

    bool PuedoDisparar()
    {
        return Time.time >= tiempoUltimoDisparo + cadencia;
    }

    void Disparar()
    {
        for (int i = 0; i < cantidadPerdigones; i++)
        {
            // Calcular dirección aleatoria dentro del ángulo de dispersión
            Vector3 dispersion = new Vector3(
                UnityEngine.Random.Range(-anguloDispersion, anguloDispersion),
                UnityEngine.Random.Range(-anguloDispersion, anguloDispersion),
                0f
            );

            Quaternion rotacionConDispersion = Quaternion.Euler(
                puntoDisparo.eulerAngles + dispersion
            );

            // Instanciar cada perdigón con su propia rotación
            GameObject perdigon = Instantiate(
                balaPrefab,
                puntoDisparo.position,
                rotacionConDispersion
            );

            // Obtener el Rigidbody y aplicar velocidad manualmente
            Rigidbody rb = perdigon.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = rotacionConDispersion * Vector3.forward * perdigon.GetComponent<BalaCaida>().speed;
            }
        }
        Cargador--;
        NotificarHUD();
    }
    public void Recargar()
    {
         
        if(Cargador < MaxMag){
            Cargador = MaxMag; //Recargamos
            CargadoresTotales--; //1 cargador menos
            recargando = true; 
            NotificarHUD();
        }
    }
    void NotificarHUD()
    {
        OnAmmoChanged?.Invoke(Cargador, MaxMag, CargadoresTotales, recargando);
    }
}