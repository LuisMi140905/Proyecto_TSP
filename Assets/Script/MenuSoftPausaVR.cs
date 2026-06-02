using UnityEngine;
using UnityEngine.AI; // Por si usas NavMeshAgent en el Minotauro

public class MenuSoftPausaVR : MonoBehaviour
{
    [Header("Configuración de Interfaz")]
    public GameObject panelPausa;
    public Transform camaraJugador;
    public float distanciaFrente = 2.5f;

    [Header("Conexión de Escenas")]
    public SceneNavigator navegador;
    public string nombreMenuInicio = "MenúInicio";

    [Header("Actores a Congelar")]
    public MonoBehaviour scriptMovimientoJugador; // El script que mueve al jugador
    public MonoBehaviour scriptIA_Minotauro;      // El script de la máquina de estados del Minotauro
    // Opcional: Si el Minotauro usa NavMeshAgent para caminar, también lo apagamos
    public NavMeshAgent agenteMinotauro;

    private bool estaPausado = false;

    void Start()
    {
        panelPausa.SetActive(false);
    }

    void Update()
    {
        // Detecta el botón Atrás o el B del control
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            if (estaPausado) ReanudarJuego();
            else PausarJuego();
        }
    }

    public void PausarJuego()
    {
        estaPausado = true;

        // 1. "Sedamos" a los actores apagando sus scripts
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = false;
        if (scriptIA_Minotauro != null) scriptIA_Minotauro.enabled = false;
        if (agenteMinotauro != null) agenteMinotauro.isStopped = true;

        // 2. Mueve el menú frente a la cara
        panelPausa.transform.position = camaraJugador.position + (camaraJugador.forward * distanciaFrente);
        panelPausa.transform.LookAt(camaraJugador);
        panelPausa.transform.Rotate(0, 180, 0);
        panelPausa.SetActive(true);

        // ¡Ya no tocamos el Time.timeScale! El mundo físico sigue vivo.
    }

    public void ReanudarJuego()
    {
        estaPausado = false;
        panelPausa.SetActive(false);

        // "Despertamos" a los actores
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = true;
        if (scriptIA_Minotauro != null) scriptIA_Minotauro.enabled = true;
        if (agenteMinotauro != null) agenteMinotauro.isStopped = false;
    }

    public void SalirAlMenu()
    {
        if (navegador != null)
        {
            navegador.LoadSceneByName(nombreMenuInicio);
        }
    }
}