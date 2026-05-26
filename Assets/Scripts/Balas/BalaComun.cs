using System;
using System.Collections;
using UnityEngine;

public class BalaComun : MonoBehaviour
{
    public static event Action<int, int, int, bool> OnAmmoChanged;

    [Header("Disparo")]
    public Transform firePoint;
    public float alcance = 100f;
    public float damage = 10f;

    [Header("Municion")]
    public int cargador;
    public int maxMag;
    public int cargadoresTotales;
    public float recargaTime;
    private float recargaContador;
    public bool recargando = false;

    [Header("Cadencia")]
    public float fireRate;
    private float nextFireTime;

    [Header("Tracer")]
    public LineRenderer tracer;
    public float tracerDuration = 0.05f;

    private Camera camara;

    void Start()
    {
        camara = Camera.main;
        cargador = maxMag;
        NotificarHUD();
        recargaContador = recargaTime;
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && cargador > 0 && Time.time >= nextFireTime && !recargando)
            Disparar();

        if (Input.GetKeyDown(KeyCode.R) && cargadoresTotales > 0)
            Recargar();
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

    void Disparar()
    {
        cargador--;
        nextFireTime = Time.time + fireRate;
        NotificarHUD();

        // Un solo Raycast para todo
        Ray rayo = camara.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit impacto;

        Vector3 destino;

        if (Physics.Raycast(rayo, out impacto, alcance))
        {
            destino = impacto.point;

            EnemyManager enemigo = impacto.collider.GetComponent<EnemyManager>();
            if (enemigo != null)
                enemigo.RecibirDanio(damage);

            // Alerta al enemigo
            MovimientoEnemigo movimiento = impacto.collider.GetComponent<MovimientoEnemigo>();
            if (movimiento != null)
                movimiento.Alertar();

            Debug.Log("Impacto en: " + impacto.collider.name);
        }
        else
        {
            destino = rayo.GetPoint(alcance);
        }

        StartCoroutine(MostrarTracer(firePoint.position, destino));
    }

    void Recargar()
    {
        if (cargador < maxMag)
        {
            recargando = true;
            cargador = maxMag;
            cargadoresTotales--;
            NotificarHUD();   
        }
    }

    void NotificarHUD()
    {
        OnAmmoChanged?.Invoke(cargador, maxMag, cargadoresTotales, recargando);
    }

    IEnumerator MostrarTracer(Vector3 origen, Vector3 destino)
    {
        tracer.SetPosition(0, origen);
        tracer.SetPosition(1, destino);
        tracer.enabled = true;
        yield return new WaitForSeconds(tracerDuration);
        tracer.enabled = false;
    }
}