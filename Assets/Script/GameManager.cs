using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración de Nivel")]
    public Transform exit;
    public Transform entrance; // Para reiniciar al jugador si lo atrapan
    public bool gameWon = false;

    // Tu referencia a la base de datos
    private DatabaseReference reference;

    // Estructura para el objeto complejo, similar a tu clase "Usuario"
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
        // Singleton para asegurar que solo haya un GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Apuntamos a la raíz de tu base de datos Firebase
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Esta función se llama desde el script de la salida cuando el jugador la toca
    public void WinGame()
    {
        if (gameWon) return;
        gameWon = true;

        // Calculamos el tiempo que le tomó escapar
        float tiempoFinal = Time.timeSinceLevelLoad;
        Debug.Log("¡Escapaste! Tiempo: " + tiempoFinal + " segundos.");

        // Subimos el récord (puedes cambiar "Jugador_1" por una variable si haces un menú de inicio)
        SubirRecord("Jugador_1", tiempoFinal);
    }

    private void SubirRecord(string nombre, float tiempo)
    {
        // 1. Creamos el registro del objeto tipo RecordJugador
        RecordJugador nuevoRecord = new RecordJugador(nombre, tiempo);

        // 2. Convertimos el objeto a JSON
        string json = JsonUtility.ToJson(nuevoRecord);

        // 3. Generamos una clave única en el nodo "RecordsLaberinto"
        string key = reference.Child("RecordsLaberinto").Push().Key;

        // 4. Enviamos el JSON a Firebase de forma asíncrona
        reference.Child("RecordsLaberinto").Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                // Confirmación idéntica a la de tu consola en la práctica
                Debug.Log("Dato registrado tipo objeto JSON: " + json);
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error al registrar el tiempo en Firebase.");
            }
        });
    }

    // Función auxiliar que usa el Minotauro para reiniciar el nivel
    public void ResetLevel(GameObject player, float delay)
    {
        Invoke(nameof(ReiniciarPosicion), delay);
    }

    private void ReiniciarPosicion()
    {
        // Reiniciamos todo para un nuevo intento
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && entrance != null)
        {
            playerObj.transform.position = entrance.position;
            
            // Reactivamos el movimiento
            ControladorVR scriptMovimiento = playerObj.GetComponent<ControladorVR>();
            if (scriptMovimiento != null) scriptMovimiento.enabled = true;
        }

        // Devolvemos al Minotauro a su guardia
        EnemyAI minotauro = Object.FindFirstObjectByType<EnemyAI>();
        if (minotauro != null)
        {
            minotauro.ResetPositionRandom();
        }
    }
}