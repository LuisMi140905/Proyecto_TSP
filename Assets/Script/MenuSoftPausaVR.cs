using UnityEngine;
// Quitamos la librería de IA porque ya no controlaremos al Minotauro desde aquí

public class MenuSoftPausaVR : MonoBehaviour
{
    [Header("Configuración de Interfaz")]
    public GameObject panelPausa;
    public Transform camaraJugador;
    public float distanciaFrente = 2.5f;

    [Header("Conexión de Escenas")]
    public SceneNavigator navegador;
    public string nombreMenuInicio = "MenúInicio";

    private bool menuActivo = false;

    void Start()
    {
        panelPausa.SetActive(false);
    }

    void Update()
    {
        // Detecta el botón Atrás (celular) o el B (control)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            if (menuActivo) ReanudarJuego();
            else MostrarMenu();
        }
    }

    public void MostrarMenu()
    {
        menuActivo = true;

        // Teletransporta el menú justo frente a tu cara
        panelPausa.transform.position = camaraJugador.position + (camaraJugador.forward * distanciaFrente);
        panelPausa.transform.LookAt(camaraJugador);
        panelPausa.transform.Rotate(0, 180, 0);
        panelPausa.SetActive(true);
    }

    // Mantenemos este nombre para que tu botón "Continuar" siga funcionando sin reconectarlo
    public void ReanudarJuego()
    {
        menuActivo = false;
        panelPausa.SetActive(false);
    }

    public void SalirAlMenu()
    {
        if (navegador != null)
        {
            navegador.LoadSceneByName(nombreMenuInicio);
        }
    }
}