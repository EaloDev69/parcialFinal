using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MovimientoEnemigo : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public float rangoPlayer;

    public Transform[] waypoints;
    public float waypointDistance = 1f;
    public int currentWaypoint= 0;


    public bool alertado = false;
    public float tiempoAlerta = 5f;      // segundos que dura la alerta
    private float contadorAlerta = 0f;
    private float distanciaParada;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
        distanciaParada = rangoPlayer * 0.75f;
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, player.position);
        if (alertado || distancia < rangoPlayer )
        {
            //jugador cerca
            Vector3 direccion = (player.position - transform.position).normalized;
            Vector3 destino = player.position - direccion * distanciaParada;
            agent.SetDestination(destino);
            if (alertado)
            {
                contadorAlerta -= Time.deltaTime;
                if (contadorAlerta <= 0f)
                    alertado = false;  // se acabó la alerta, vuelve a patrullar
            }
        }
        else
        {
            //volver a patrullaje
            if(!agent.pathPending && agent.remainingDistance <= waypointDistance)
            {
                GoToNextWaypoint();
            }
        }
    }
    void GoToNextWaypoint()
    {
        currentWaypoint++;//suma de contador

        if(currentWaypoint >= waypoints.Length)
        {
            currentWaypoint = 0;//si supera longitud, vuelve a 0
        }

        agent.SetDestination(waypoints[currentWaypoint].position);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bala"))
        {
           Alertar();
        }
    }
    public void Alertar()
    {
        alertado = true;
        contadorAlerta = tiempoAlerta;
    }
}
