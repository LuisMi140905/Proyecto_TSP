using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Datos del Jugador")]
    public string nombreActualJugador = "Invitado";

    [Header("Configuración de Nivel")]
    public Transform exit;
    public Transform entrance;
    public bool gameWon = false;

    [Header("Nombres de Escenas (Videos)")]
    public string escenaVictoria = "Salida";
    public string escenaDerrota = "Perdiste";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EstablecerNombreJugador(string nombre)
    {
        if (!string.IsNullOrEmpty(nombre))
        {
            nombreActualJugador = nombre;
        }
    }

    public void WinGame()
    {
        if (gameWon) return;
        gameWon = true;

        float tiempoFinal = Time.timeSinceLevelLoad;
        Debug.Log("¡Escapaste! Tiempo: " + tiempoFinal + " segundos. Jugador: " + nombreActualJugador);

        // Sin Firebase: Cargamos directamente la escena de victoria
        SceneManager.LoadScene(escenaVictoria);
    }

    public void ResetLevel(GameObject player, float delay)
    {
        StartCoroutine(RutinaReinicioNivel(delay));
    }

    private System.Collections.IEnumerator RutinaReinicioNivel(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && entrance != null)
        {
            // 1. Apagamos tu script Prueba3 temporalmente
            Prueba3 scriptMovimiento = playerObj.GetComponent<Prueba3>();
            if (scriptMovimiento != null) scriptMovimiento.enabled = false;

            // 2. En lugar de CharacterController, usamos tu Rigidbody.
            // Lo hacemos "Cinemático" un milisegundo para que el motor de físicas no choque al teletransportarlo.
            Rigidbody rb = playerObj.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            // 3. Teletransportamos al jugador a la entrada
            playerObj.transform.position = entrance.position;

            // 4. Devolvemos todo a la normalidad
            if (rb != null) rb.isKinematic = false;
            if (scriptMovimiento != null) scriptMovimiento.enabled = true;
        }

        // Reiniciamos al minotauro
        EnemyAI minotauro = Object.FindFirstObjectByType<EnemyAI>();
        if (minotauro != null)
        {
            minotauro.ResetPositionRandom();
        }
    }

    private void CargarEscenaPerdiste()
    {
        SceneManager.LoadScene(escenaDerrota);
    }
}