using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemySniperState
{
    Patrullaje,
    Siguiendo,
    Alertado,
    Cobertura
}

public class MovimientoSniper : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] public Transform[] patrolPoints;
    [SerializeField] public Transform[] coverPoints;
    [Header("Settings")]
    [SerializeField]private float patrolWaitTime = 2f;
    [SerializeField]private float  stopAtDistance = 0.5f;
    [SerializeField]public float detectionRange = 5f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float losePlayerTime = 3f;
    private NavMeshAgent agent;
    private int puntoActual;
    private bool esperando;
    private EnemySniperState state = EnemySniperState.Patrullaje;
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
            case EnemySniperState.Patrullaje:
                Patrol();
                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    state = EnemySniperState.Siguiendo;
                }

                break;

            case EnemySniperState.Siguiendo:
                FollowPlayer();
                if (!CanSeePlayer())
                {
                    timeSinceLostPlayer  += Time.deltaTime;
                    if(timeSinceLostPlayer >= losePlayerTime)
                    {
                        state = EnemySniperState.Patrullaje;
                        GoToClosestPatrolPoint();
                    }
                }
                else
                {
                    timeSinceLostPlayer   = 0f;
                }
                break;
            case EnemySniperState.Alertado:
                FollowPlayer(); // se mueve hacia donde escuchó algo
                if (CanSeePlayer())
                {
                    state = EnemySniperState.Siguiendo; // si lo ve, pasa a perseguir
                }
                timeSinceLostPlayer += Time.deltaTime;
                if (timeSinceLostPlayer >= losePlayerTime)
                {
                    state = EnemySniperState.Patrullaje;
                    timeSinceLostPlayer = 0f;
                    GoToClosestPatrolPoint();
                }
                break;
            case EnemySniperState.Cobertura:
                agent.isStopped = true;
                if (distanceToPlayer < detectionRange * 0.5f)
                {
                    agent.isStopped = false;
                    GoToClosestCoverPoint(); // ← cambiado
                    state = EnemySniperState.Siguiendo;
                }
                if (!CanSeePlayer())
                {
                    timeSinceLostPlayer += Time.deltaTime;
                    if (timeSinceLostPlayer >= losePlayerTime)
                    {
                        agent.isStopped = false;
                        state = EnemySniperState.Patrullaje;
                        GoToClosestPatrolPoint();
                    }
                }
                else { timeSinceLostPlayer = 0f; }
                break;
        }
        
    }
    public void Alertar()
{
    if (state == EnemySniperState.Patrullaje)
    {
        timeSinceLostPlayer = 0f;
        GoToClosestCoverPoint(); // ← agregar esto para que se mueva al cubrirse
        state = EnemySniperState.Cobertura;
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
    void GoToClosestCoverPoint()
    {
        if (coverPoints.Length == 0)
        {
            GoToClosestPatrolPoint(); // fallback si no hay puntos de cobertura
            return;
        }
        var closestIndex = 0;
        var closestDistance = float.MaxValue;

        for (int i = 0; i < coverPoints.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, coverPoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        agent.SetDestination(coverPoints[closestIndex].position);
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
