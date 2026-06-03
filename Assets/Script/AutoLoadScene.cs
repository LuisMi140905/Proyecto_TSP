using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadScene : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string nombreEscena;
    [SerializeField] private float tiempoEspera = 3f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(tiempoEspera);

        if (!string.IsNullOrEmpty(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError("No se ha asignado el nombre de la escena.");
        }
    }
}