using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias de Nivel")]
    public Transform entrance;
    public Transform exit;

    public bool gameWon { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void WinGame()
    {
        gameWon = true;
        Debug.Log("¡Escapaste del Minotauro!");
    }

    public void ResetLevel(GameObject player, float delay)
    {
        StartCoroutine(ResetSequence(player, delay));
    }

    private IEnumerator ResetSequence(GameObject player, float delay)
    {
        yield return new WaitForSeconds(delay);

        var agent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        player.transform.position = entrance.position;

        if (agent != null) agent.enabled = true;

        // Reiniciar a todos los enemigos en la escena
        EnemyAI[] allEnemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        foreach (EnemyAI enemy in allEnemies)
        {
            enemy.ResetPosition(entrance.position);
        }
    }
}