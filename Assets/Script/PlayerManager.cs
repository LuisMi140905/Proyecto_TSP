using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Configuración")]
    public float exitRange = 2f;

    void Update()
    {
        // Evitar errores si el GameManager no está listo o el juego ya terminó
        if (GameManager.Instance == null || GameManager.Instance.gameWon) return;

        // Verificar si llegamos a la salida
        float distanceToExit = Vector3.Distance(transform.position, GameManager.Instance.exit.position);

        if (distanceToExit < exitRange)
        {
            GameManager.Instance.WinGame();
        }
    }
}