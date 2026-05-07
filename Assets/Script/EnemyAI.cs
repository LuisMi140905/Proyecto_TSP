using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Detección")]
    public float detectionRange = 2f;
    public float viewDistance = 12f;
    public float viewAngle = 120f;
    public float sphereCastRadius = 0.1f;
    public LayerMask obstacleMask;

    [Header("IA y Memoria")]
    public float patrolRadius = 10f;
    public float waitTimeAtPoint = 2f;
    public float memoryTime = 4f;

    [Header("Velocidades")]
    public float chaseSpeed = 3.5f;
    public float patrolSpeed = 1.8f;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform targetPlayer;

    private float waitTimer;
    private float memoryTimer;
    private bool isCatching = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        // Busca automáticamente al jugador por su Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) targetPlayer = playerObj.transform;
    }

    void Update()
    {
        if (GameManager.Instance.gameWon || isCatching || targetPlayer == null) return;

        if (anim != null) anim.SetFloat("Speed", agent.velocity.magnitude);

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);

        // Lógica de Atrapado
        if (distanceToPlayer < detectionRange)
        {
            CatchPlayer();
            return;
        }

        // Lógica de Máquina de Estados
        if (CanSeePlayer())
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.SetDestination(targetPlayer.position);
            memoryTimer = memoryTime;
        }
        else if (memoryTimer > 0)
        {
            memoryTimer -= Time.deltaTime;
            if (!agent.pathPending && agent.remainingDistance < 1.5f)
            {
                memoryTimer = 0;
            }
        }
        else
        {
            PerformPatrol();
        }
    }

    private void CatchPlayer()
    {
        isCatching = true;
        agent.isStopped = true;
        agent.ResetPath();

        if (anim != null) anim.SetTrigger("Attack");

        // Le avisa al GameManager que reinicie el nivel después de 1.5 segundos
        GameManager.Instance.ResetLevel(targetPlayer.gameObject, 1.5f);
    }

    public void ResetPosition(Vector3 entrancePos)
    {
        agent.enabled = false;

        // Lo mandamos a un punto aleatorio lejos de la entrada
        Vector3 randomPos = Random.insideUnitSphere * 50f;
        randomPos += entrancePos;
        NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 50f, NavMesh.AllAreas);
        transform.position = hit.position;

        agent.enabled = true;
        agent.ResetPath();
        agent.speed = patrolSpeed;
        memoryTimer = 0;
        isCatching = false;
    }

    private void PerformPatrol()
    {
        agent.speed = patrolSpeed;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                Vector3 randomPos = Random.insideUnitSphere * patrolRadius;
                randomPos += transform.position;
                NavMesh.SamplePosition(randomPos, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas);
                agent.SetDestination(hit.position);
                waitTimer = 0;
            }
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 eyePosition = transform.position + Vector3.up * 1.5f;
        Vector3 targetPos = targetPlayer.position + Vector3.up * 1.0f;
        Vector3 dirToPlayer = (targetPos - eyePosition).normalized;
        float distToPlayer = Vector3.Distance(eyePosition, targetPos);

        if (distToPlayer < viewDistance)
        {
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
            {
                if (!Physics.SphereCast(eyePosition, sphereCastRadius, dirToPlayer, out RaycastHit hit, distToPlayer, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
