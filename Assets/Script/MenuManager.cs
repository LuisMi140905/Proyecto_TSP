using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Conexión con Navegador")]
    public SceneNavigator navegador;

    [Header("Nombres de Escenas")]
    public string escenaLaberinto = "Nivel1";
    public string escenaMarcadores = "EscenaMarcadores";

    // --- FUNCIONES PARA LOS BOTONES ---

    public void BotonJugar()
    {
        Debug.Log("Iniciando el laberinto...");
        if (navegador != null) navegador.LoadSceneByName(escenaLaberinto);
    }

    public void BotonMarcadores()
    {
        Debug.Log("Abriendo marcadores...");
        if (navegador != null) navegador.LoadSceneByName(escenaMarcadores);
    }

    public void BotonOpciones()
    {
        // Aquí pondremos la lógica de tu panel de opciones más adelante
        Debug.Log("Opciones en construcción.");
    }

    public void BotonSalir()
    {
        if (navegador != null) navegador.ExitGame();
    }
}