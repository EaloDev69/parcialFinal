using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisparoPrefab : MonoBehaviour
{
    public Transform firePoint;
    public GameObject balaPrefab;
    public float speed = 20f;
    public float municionMax = 10;
    private bool tieneBalas = true;
    private float recarga;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && municionMax > 0 && tieneBalas == true )
        {
            Disparar();
            Debug.Log("sonido de disparo");
            municionMax--;
        }

        if(municionMax==0)
        {
            tieneBalas = false;
        }

        if (Input.GetButtonDown("Fire2") && municionMax != 10) 
        {
            recarga = 10;
            municionMax = recarga;
            tieneBalas = true;
        }
    }

    public void Disparar()
    {
        Instantiate(balaPrefab, firePoint.position, firePoint.rotation);
    }
}
