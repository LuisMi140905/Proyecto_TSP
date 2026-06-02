using UnityEngine;
using UnityEngine.EventSystems; // Necesario para detectar el puntero
using UnityEngine.UI;           // Necesario para interactuar con el botón

[RequireComponent(typeof(Button))]
public class BotonVR : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración de Mirada")]
    public float tiempoParaClic = 3.0f;

    private float contadorTiempo = 0f;
    private bool estaMirando = false;
    private Button boton;

    void Start()
    {
        // Vinculamos el componente Button que ya tiene tu objeto
        boton = GetComponent<Button>();
    }

    void Update()
    {
        // Si el láser del jugador está sobre el botón, empezamos a contar
        if (estaMirando)
        {
            contadorTiempo += Time.deltaTime;

            if (contadorTiempo >= tiempoParaClic)
            {
                // ¡Llegamos a los 3 segundos! Disparamos el botón.
                boton.onClick.Invoke();

                // Apagamos la mirada para que no haga clic 100 veces por segundo
                estaMirando = false;
                contadorTiempo = 0f;
            }
        }
    }

    // Esta función se activa automáticamente cuando el Reticle entra al botón
    public void OnPointerEnter(PointerEventData eventData)
    {
        estaMirando = true;
        contadorTiempo = 0f; // Reiniciamos el reloj por si lo miró a medias antes
        Debug.Log("Mirando el botón: " + gameObject.name);
    }

    // Esta función se activa cuando el Reticle sale del botón
    public void OnPointerExit(PointerEventData eventData)
    {
        estaMirando = false;
        contadorTiempo = 0f; // Cancelamos el clic
    }
}