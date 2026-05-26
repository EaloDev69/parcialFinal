using UnityEngine;

public class DisparoEnemigo : MonoBehaviour
{
    public Transform firePoint; //Usen la camara por ahora
    public GameObject bala; //Prefab de bala
    public GameObject Jugador;
    public int Cargador; 
    public int MaxMag; //Modificar segun el arma antes de play
    public int CargadoresTotales; //Modificar segun el arma antes de play
    public float fireRate;
    private MovimientoEnemigo me;
    private float tiempoDisparo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cargador = MaxMag;
        me = GetComponentInParent<MovimientoEnemigo>();
    }

    // Update is called once per frame
    void Update()
    {
        float distancia =Vector3.Distance(Jugador.transform.position, transform.position);

        if (distancia < me.rangoPlayer)
        {
            tiempoDisparo += Time.deltaTime;

            if (tiempoDisparo > fireRate)
            {
                tiempoDisparo = 0;
                Disparar();
            }
        }
    }

     void Disparar()
    {
        GameObject balin = Instantiate(bala, firePoint.position, firePoint.rotation);
        Destroy(balin, 3f);
        if(Cargador < 0)
        {
            Recargar();
        }
    }
     public void Recargar()
    {
        if(Cargador < MaxMag){
        Cargador = MaxMag; //Recargamos
        CargadoresTotales--; //1 cargador menos
        }
    }
}
