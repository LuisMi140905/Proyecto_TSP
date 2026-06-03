using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Datos del Jugador")]
    // Aquí está tu variable libre. Tiene "Invitado" por defecto por si juegan sin poner nombre.
    public string nombreActualJugador = "Invitado";

    [Header("Configuración de Nivel")]
    public Transform exit;
    public Transform entrance;
    public bool gameWon = false;

    [Header("Nombres de Escenas (Videos)")]
    public string escenaVictoria = "Salida";
    public string escenaDerrota = "Perdiste";

    private DatabaseReference reference;

    [System.Serializable]
    public class RecordJugador
    {
        public string nombreJugador;
        public float tiempoSegundos;

        public RecordJugador(string nombre, float tiempo)
        {
            nombreJugador = nombre;
            tiempoSegundos = tiempo;
        }
    }

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

        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // --- NUEVA FUNCIÓN PARA EL FUTURO ---
    // Cuando hagas tu Canvas para pedir el nombre, conecta el evento "On End Edit" del InputField a esta función.
    public void EstablecerNombreJugador(string nombre)
    {
        if (!string.IsNullOrEmpty(nombre))
        {
            nombreActualJugador = nombre;
        }
    }
    // ------------------------------------

    public void WinGame()
    {
        if (gameWon) return;
        gameWon = true;

        float tiempoFinal = Time.timeSinceLevelLoad;
        Debug.Log("¡Escapaste! Tiempo: " + tiempoFinal + " segundos. Jugador: " + nombreActualJugador);

        // Ahora usamos la variable en lugar del texto fijo "Jugador_1"
        SubirRecord(nombreActualJugador, tiempoFinal);
    }

    private void SubirRecord(string nombre, float tiempo)
    {
        RecordJugador nuevoRecord = new RecordJugador(nombre, tiempo);
        string json = JsonUtility.ToJson(nuevoRecord);
        string key = reference.Child("RecordsLaberinto").Push().Key;

        reference.Child("RecordsLaberinto").Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Dato registrado en Firebase: " + json);
                SceneManager.LoadScene(escenaVictoria);
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error al registrar en Firebase. Cargando escena por seguridad...");
                SceneManager.LoadScene(escenaVictoria);
            }
        });
    }

    public void ResetLevel(GameObject player, float delay)
    {
        // Usamos una corrutina en lugar de Invoke para ignorar pausas de tiempo
        StartCoroutine(RutinaReinicioNivel(delay));
    }

    private System.Collections.IEnumerator RutinaReinicioNivel(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && entrance != null)
        {
            // Apagamos momentáneamente el CharacterController (o Rigidbody) si tienes uno para evitar conflictos de físicas al teletransportar
            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            playerObj.transform.position = entrance.position;

            // Si tenías un controlador VR apagado, lo prendemos
            //ControladorVR scriptMovimiento = playerObj.GetComponent<ControladorVR>();
            //if (scriptMovimiento != null) scriptMovimiento.enabled = true;

            if (cc != null) cc.enabled = true;
        }

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