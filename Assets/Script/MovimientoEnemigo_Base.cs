using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovimientoEnemigo_Base : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public int rangoPlayer = 10;
    
    
    void Start()
    {
        // Obtiene la referencia al componente NavMeshAgent al iniciar
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distancia = Vector3.Distance(player.position, transform.position);
        
        if(distancia < rangoPlayer)
        {
          agent.SetDestination(player.position);
        }
    }
    
  
}