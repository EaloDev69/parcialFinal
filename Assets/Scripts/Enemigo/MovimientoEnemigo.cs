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
    private Transform player;
    [SerializeField] public Transform[] patrolPoints;

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
    private EnemyState state = EnemyState.Patrullaje;
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
            case EnemyState.Patrullaje:
                Patrol();
                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    state = EnemyState.Siguiendo;
                }
                break;

            case EnemyState.Siguiendo:
                // Reset para que balas/alertas futuras lo puedan volver a activar
                alreadyPropagated = false;
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

            case EnemyState.Alertado:
                agent.SetDestination(alertTarget);
                if (CanSeePlayer())
                {
                    state = EnemyState.Siguiendo;
                    break;
                }
                // El timer solo corre cuando ya llegó al destino, no mientras camina
                if (!agent.pathPending && agent.remainingDistance <= stopAtDistance)
                {
                    timeSinceLostPlayer += Time.deltaTime;
                    if (timeSinceLostPlayer >= losePlayerTime)
                    {
                        ReturnToPatrol();
                    }
                }
                else
                {
                    timeSinceLostPlayer = 0f; // sigue caminando, resetea el timer
                }
                break;
        }
    }

    // Llamado desde afuera (ruido, disparo, etc.)
    public void Alertar(Vector3 noiseOrigin, int generation = 0)
    {
        // Si ya propagó en esta alerta, no vuelve a hacerlo (evita ciclo)
        if (alreadyPropagated) return;

        // Si ya está persiguiendo al jugador, no lo interrumpas
        if (state == EnemyState.Siguiendo) return;

        // Si la generación supera el límite, no propaga más pero SÍ se alerta
        // (el enemigo reacciona, solo deja de reenviar la señal)
        alertTarget = noiseOrigin;
        alertGeneration = generation;
        timeSinceLostPlayer = 0f;
        state = EnemyState.Alertado;
        alreadyPropagated = true;

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
            if (neighbor.gameObject == gameObject) continue; // no llamarse a sí mismo

            var enemigo = neighbor.GetComponent<MovimientoEnemigo>();
            enemigo?.Alertar(alertTarget, generation + 1);
        }
    }

    // Centraliza el reset completo al volver a patrullar
    private void ReturnToPatrol()
    {
        state = EnemyState.Patrullaje;
        timeSinceLostPlayer = 0f;
        alertGeneration = 0;
        alreadyPropagated = false;
        esperando = false;
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

    void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.ResetPath();
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

        agent.ResetPath();
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