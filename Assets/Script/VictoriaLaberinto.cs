using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoriaLaberinto : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreEscenaVictoria = "Salida"; // Pon el nombre exacto de tu nueva escena

    private void OnTriggerEnter(Collider other)
    {
        // Revisamos si el objeto que cruzó la salida tiene la etiqueta "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Meta alcanzada! Cargando video de victoria...");
            SceneManager.LoadScene(nombreEscenaVictoria);
        }
    }
}