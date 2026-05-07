using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Collections;

public class AIManager : MonoBehaviour
{
    [Header("Configuración de Objetos")]
    [SerializeField] public GameObject player;
    [SerializeField] public Transform entrance;
    [SerializeField] public Transform exit;

    [Header("Parámetros de Detección")]
    public float detectionRange = 2f;
    public float viewDistance = 12f;
    public float viewAngle = 120f;
    public float sphereCastRadius = 0.1f;
    public LayerMask obstacleMask;

    [Header("Parámetros de IA")]
    public float patrolRadius = 10f;
    public float waitTimeAtPoint = 2f;
    public float memoryTime = 4f;

    [Header("Velocidades")]
    public float chaseSpeed = 3.5f;
    public float patrolSpeed = 1.8f;

    private List<NavMeshAgent> agents = new List<NavMeshAgent>();
    private Dictionary<NavMeshAgent, float> agentWaitTimers = new Dictionary<NavMeshAgent, float>();
    private Dictionary<NavMeshAgent, float> memoryTimers = new Dictionary<NavMeshAgent, float>();
    private Vector3 entrancePos;
    private bool gameWon = false;
    private bool isResetting = false;

    void Start()
    {
        entrancePos = entrance.position;
        FindAllEnemies();
    }

    void Update()
    {
        if (gameWon || isResetting) return;

        Vector3 playerPos = player.transform.position;

        foreach (var agent in agents)
        {
            if (!agent.enabled) continue;

            Animator enemyAnim = agent.GetComponentInChildren<Animator>();
            if (enemyAnim != null) enemyAnim.SetFloat("Speed", agent.velocity.magnitude);

            if (Vector3.Distance(agent.transform.position, playerPos) < detectionRange)
            {
                StartCoroutine(HandleCatchSequence(agent, enemyAnim));
                return;
            }

            if (CanSeePlayer(agent))
            {
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                agent.SetDestination(playerPos);
                memoryTimers[agent] = memoryTime;
            }
            else if (memoryTimers[agent] > 0)
            {
                memoryTimers[agent] -= Time.deltaTime;

                if (!agent.pathPending && agent.remainingDistance < 1.5f)
                {
                    memoryTimers[agent] = 0;
                }
            }
            else
            {
                PerformPatrol(agent);
            }
        }

        if (Vector3.Distance(playerPos, exit.position) < 2f)
        {
            gameWon = true;
            Debug.Log("¡Escapaste!");
        }
    }

    IEnumerator HandleCatchSequence(NavMeshAgent agent, Animator anim)
    {
        isResetting = true;
        agent.isStopped = true;
        agent.ResetPath();

        if (anim != null) anim.SetTrigger("Attack");

        yield return new WaitForSeconds(1.5f);

        TeleportPlayerToEntrance();
        RelocateAllNPC();

        agent.isStopped = false;
        isResetting = false;
    }

    void PerformPatrol(NavMeshAgent agent)
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agentWaitTimers[agent] += Time.deltaTime;

            if (agentWaitTimers[agent] >= waitTimeAtPoint)
            {
                Vector3 newDestination = GetRandomPointNear(agent.transform.position, patrolRadius);
                agent.SetDestination(newDestination);
                agentWaitTimers[agent] = 0;
            }
        }
    }

    Vector3 GetRandomPointNear(Vector3 center, float distance)
    {
        Vector3 randomPos = Random.insideUnitSphere * distance;
        randomPos += center;
        NavMesh.SamplePosition(randomPos, out NavMeshHit hit, distance, NavMesh.AllAreas);
        return hit.position;
    }

    bool CanSeePlayer(NavMeshAgent agent)
    {
        Vector3 eyePosition = agent.transform.position + Vector3.up * 1.5f;
        Vector3 targetPosition = player.transform.position + Vector3.up * 1.0f;
        Vector3 dirToPlayer = (targetPosition - eyePosition).normalized;
        float distToPlayer = Vector3.Distance(eyePosition, targetPosition);

        if (distToPlayer < viewDistance)
        {
            if (Vector3.Angle(agent.transform.forward, dirToPlayer) < viewAngle / 2f)
            {
                if (!Physics.SphereCast(eyePosition, sphereCastRadius, dirToPlayer, out RaycastHit hit, distToPlayer, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void TeleportPlayerToEntrance()
    {
        var cc = player.GetComponent<NavMeshAgent>();
        if (cc != null) cc.enabled = false;
        player.transform.position = entrancePos;
        if (cc != null) cc.enabled = true;
    }

    void RelocateAllNPC()
    {
        foreach (var agent in agents)
        {
            agent.enabled = false;
            agent.transform.position = GetRandomPointNear(entrancePos, 50f);
            agent.enabled = true;

            if (agent.gameObject.activeSelf)
            {
                agent.ResetPath();
                agent.speed = patrolSpeed;
                memoryTimers[agent] = 0;
            }
        }
    }

    void FindAllEnemies()
    {
        agents.Clear();
        agentWaitTimers.Clear();
        memoryTimers.Clear();
        foreach (var agent in FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None))
        {
            if (agent.CompareTag("Enemy"))
            {
                agents.Add(agent);
                agentWaitTimers.Add(agent, 0f);
                memoryTimers.Add(agent, 0f);
            }
        }
    }
}