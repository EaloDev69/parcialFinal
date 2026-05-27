using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrullaje,
    Siguiendo,
    Alertado
}

public class MovimientoEnemigo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] public Transform[] patrolPoints;
    [Header("Settings")]
    [SerializeField]private float patrolWaitTime = 2f;
    [SerializeField]private float  stopAtDistance = 0.5f;
    [SerializeField]public float detectionRange = 5f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float losePlayerTime = 3f;
    private NavMeshAgent agent;
    private int puntoActual;
    private bool esperando;
    private EnemyState state = EnemyState.Patrullaje;
    private float timeSinceLostPlayer;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

    }
    void Start()
    {
        GoToNextPatrolPoint();
    }
    void Update()
    {
        var distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (state)
        {
            case EnemyState.Patrullaje:
                Patrol();
                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    state = EnemyState.Siguiendo;
                }

                break;

            case EnemyState.Siguiendo:
                FollowPlayer();
                if (!CanSeePlayer())
                {
                    timeSinceLostPlayer  += Time.deltaTime;
                    if(timeSinceLostPlayer >= losePlayerTime)
                    {
                        state = EnemyState.Patrullaje;
                        GoToClosestPatrolPoint();
                    }
                }
                else
                {
                    timeSinceLostPlayer   = 0f;
                }
                break;
            case EnemyState.Alertado:
                FollowPlayer(); // se mueve hacia donde escuchó algo
                if (CanSeePlayer())
                {
                    state = EnemyState.Siguiendo; // si lo ve, pasa a perseguir
                }
                timeSinceLostPlayer += Time.deltaTime;
                if (timeSinceLostPlayer >= losePlayerTime)
                {
                    state = EnemyState.Patrullaje;
                    timeSinceLostPlayer = 0f;
                    GoToClosestPatrolPoint();
                }
                break;
        }
        
    }
    public void Alertar()
{
    if (state == EnemyState.Patrullaje) // no interrumpir si ya está persiguiendo
    {
        timeSinceLostPlayer = 0f;
        state = EnemyState.Alertado;
    }
}
    void FollowPlayer()
    {
        agent.SetDestination(player.position);
    }
    void Patrol()
    {
        if (esperando) return;
        if(!agent.pathPending  && agent.remainingDistance <= stopAtDistance)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }
    private IEnumerator WaitAtPatrolPoint()
    {
        esperando = true;
        agent.isStopped =true;

        yield return new WaitForSeconds(patrolWaitTime);

        agent.isStopped = false;
        GoToNextPatrolPoint();
        esperando = false;
    }
    void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        var closestIndex = 0;
        var closestDistance = float.MaxValue;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
            
        }
        puntoActual = closestIndex;
        agent.SetDestination(patrolPoints[puntoActual].position);
    }
    void GoToNextPatrolPoint()
    {
        if(patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[puntoActual].position);
        puntoActual = (puntoActual + 1) % patrolPoints.Length;
    }
    
    bool CanSeePlayer()
    {
        return IsFacingPlayer() && HasClearPathToPlayer();
    }

    bool IsFacingPlayer()
    {
        var dirToPlayer = (player.position - transform.position).normalized;
        var angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle <= viewAngle / 2f;
    }
    bool HasClearPathToPlayer()
    {
        var  dirToPlayer = player.position - transform.position;
        if(Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, dirToPlayer.magnitude))
        {
            return hit.transform == player;
        }
        return true;
    }
}
