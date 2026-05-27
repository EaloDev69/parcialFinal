using UnityEngine;

public class DisparoEnemigo : MonoBehaviour
{
    public Transform arma;
    public Transform firePoint; //Usen la camara por ahora
    public GameObject bala; //Prefab de bala
    public GameObject Jugador;
    public int Cargador; 
    public int MaxMag; //Modificar segun el arma antes de play
    public int CargadoresTotales; //Modificar segun el arma antes de play
    public float fireRate;
    private MovimientoEnemigo me;
    private float tiempoDisparo;
    [SerializeField] private float anguloVertical = 60f;
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

        if (distancia < me.detectionRange)
        {
            ApuntarJugador();
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
        if(Cargador <= 0)
        {
            Recargar();
        }
    }
     public void Recargar()
    {
        if(CargadoresTotales <= 0) return;
        if(Cargador < MaxMag){
            Cargador = MaxMag; //Recargamos
            CargadoresTotales--; //1 cargador menos
        }
    }
    void ApuntarJugador()
    {
        Vector3 direccion = Jugador.transform.position - arma.position;
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

        Vector3 euler = rotacionObjetivo.eulerAngles;
        euler.x = Mathf.Clamp(euler.x > 180 ? euler.x - 360 : euler.x, -anguloVertical, anguloVertical);
        euler.y = 0f;
        euler.z = 0f;

        arma.localRotation = Quaternion.Euler(euler);
    }
}
