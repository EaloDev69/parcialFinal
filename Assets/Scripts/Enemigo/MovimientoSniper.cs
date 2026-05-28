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
    private Transform player;
    [SerializeField] public Transform[] patrolPoints;
    [SerializeField] public Transform[] coverPoints;

    [Header("Settings")]
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float stopAtDistance = 0.5f;
    [SerializeField] public float detectionRange = 5f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float losePlayerTime = 3f;

    [Header("Alert Propagation")]
    [SerializeField] private float alertPropagationRadius = 6f;
    [SerializeField] private int maxAlertGenerations = 5;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float propagationDelay = 0.2f;

    private NavMeshAgent agent;
    private int puntoActual;
    private bool esperando;
    private EnemySniperState state = EnemySniperState.Patrullaje;
    private float timeSinceLostPlayer;

    // --- Propagación ---
    private int alertGeneration = 0;
    private bool alreadyPropagated = false;
    private Vector3 alertTarget;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
        player = GameObject.FindWithTag("Player").transform;
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
                    GoToClosestCoverPoint();
                    state = EnemySniperState.Cobertura;
                }
                break;

            case EnemySniperState.Siguiendo:
                FollowPlayer();
                if (!CanSeePlayer())
                {
                    timeSinceLostPlayer += Time.deltaTime;
                    if (timeSinceLostPlayer >= losePlayerTime)
                    {
                        ReturnToPatrol();
                    }
                }
                else
                {
                    timeSinceLostPlayer = 0f;
                }
                break;

            case EnemySniperState.Alertado:
                // Va a investigar el origen del ruido, no al jugador
                agent.SetDestination(alertTarget);
                if (CanSeePlayer())
                {
                    // Si lo ve mientras investiga, se cubre en lugar de perseguir
                    GoToClosestCoverPoint();
                    state = EnemySniperState.Cobertura;
                }
                timeSinceLostPlayer += Time.deltaTime;
                if (timeSinceLostPlayer >= losePlayerTime)
                {
                    ReturnToPatrol();
                }
                break;

            case EnemySniperState.Cobertura:
                agent.isStopped = true;
                if (distanceToPlayer < detectionRange * 0.5f)
                {
                    // El jugador se acercó demasiado, busca nueva cobertura
                    agent.isStopped = false;
                    GoToClosestCoverPoint();
                    state = EnemySniperState.Siguiendo;
                }
                if (!CanSeePlayer())
                {
                    timeSinceLostPlayer += Time.deltaTime;
                    if (timeSinceLostPlayer >= losePlayerTime)
                    {
                        agent.isStopped = false;
                        ReturnToPatrol();
                    }
                }
                else
                {
                    timeSinceLostPlayer = 0f;
                }
                break;
        }
    }

    // Acepta tanto llamadas externas (generation 0) como propagación en cadena
    public void Alertar(Vector3 noiseOrigin, int generation = 0)
    {
        if (alreadyPropagated) return;
        if (state == EnemySniperState.Siguiendo || state == EnemySniperState.Cobertura) return;

        alertTarget = noiseOrigin;
        alertGeneration = generation;
        timeSinceLostPlayer = 0f;
        alreadyPropagated = true;

        // El sniper se cubre, no va a investigar el ruido directamente
        GoToClosestCoverPoint();
        state = EnemySniperState.Cobertura;

        if (generation < maxAlertGenerations)
        {
            StartCoroutine(PropagateAlert(generation));
        }
    }

    private IEnumerator PropagateAlert(int generation)
    {
        yield return new WaitForSeconds(propagationDelay);

        var neighbors = Physics.OverlapSphere(transform.position, alertPropagationRadius, enemyLayer);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject) continue;

            // Propaga a cualquier tipo de enemigo que tenga Alertar()
            neighbor.GetComponent<MovimientoEnemigo>()?.Alertar(alertTarget, generation + 1);
            neighbor.GetComponent<MovimientoSniper>()?.Alertar(alertTarget, generation + 1);
        }
    }

    private void ReturnToPatrol()
    {
        state = EnemySniperState.Patrullaje;
        timeSinceLostPlayer = 0f;
        alertGeneration = 0;
        alreadyPropagated = false;
        esperando = false;
        agent.isStopped = false;
        StopAllCoroutines();
        GoToClosestPatrolPoint();
    }

    void FollowPlayer()
    {
        agent.SetDestination(player.position);
    }

    void Patrol()
    {
        if (esperando) return;
        if (!agent.pathPending && agent.remainingDistance <= stopAtDistance)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        esperando = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        agent.isStopped = false;
        GoToNextPatrolPoint();
        esperando = false;
    }

    void GoToClosestCoverPoint()
    {
        if (coverPoints.Length == 0)
        {
            GoToClosestPatrolPoint();
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
        if (patrolPoints.Length == 0) return;

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
        var dirToPlayer = player.position - transform.position;
        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, dirToPlayer.magnitude))
        {
            return hit.transform == player;
        }
        return true;
    }
}