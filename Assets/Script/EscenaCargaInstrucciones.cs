using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadLevelManager : MonoBehaviour
{
    public Image imagenRelleno;
    public GameObject botonEntrar;

    [Header("Configuración")]
    public float tiempoParaLeer = 4f; // Segundos que tarda en llenarse la barra

    void Start()
    {
        botonEntrar.SetActive(false); // Mantenemos el botón oculto mientras leen
        StartCoroutine(LlenarBarra());
    }

    IEnumerator LlenarBarra()
    {
        float tiempoActual = 0f;

        // Simulamos la carga visual en un hilo secundario
        while (tiempoActual < tiempoParaLeer)
        {
            tiempoActual += Time.deltaTime;
            imagenRelleno.fillAmount = tiempoActual / tiempoParaLeer;
            yield return null;
        }

        imagenRelleno.fillAmount = 1f;
        botonEntrar.SetActive(true); // ¡El usuario ya puede usar el botón!
    }
}