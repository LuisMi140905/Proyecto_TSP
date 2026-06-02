using UnityEngine;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI Marcadores")]
    public TextMeshProUGUI textoMarcadores; // Arrastra aquí tu componente de texto del Canvas

    private DatabaseReference reference;

    void Start()
    {
        // 1. Inicializamos la conexión a Firebase
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        // 2. Ponemos un texto de espera para el usuario
        if (textoMarcadores != null)
        {
            textoMarcadores.text = "Conectando con la base de datos...\nCargando tiempos...";
        }

        // 3. Disparamos la descarga
        DescargarMejoresTiempos();
    }

    public void DescargarMejoresTiempos()
    {
        // Buscamos en tu nodo exacto "RecordsLaberinto", ordenamos de menor a mayor y pedimos los 5 mejores
        reference.Child("RecordsLaberinto")
                 .OrderByChild("tiempoSegundos")
                 .LimitToFirst(5)
                 .GetValueAsync().ContinueWithOnMainThread(task =>
                 {
                     if (task.IsFaulted)
                     {
                         Debug.LogError("Error al conectar con Firebase: " + task.Exception);
                         if (textoMarcadores != null) textoMarcadores.text = "Error de conexión. Intenta más tarde.";
                         return;
                     }

                     if (task.IsCompleted)
                     {
                         DataSnapshot snapshot = task.Result;

                         // Preparamos el título del texto
                         string textoFinal = "--- TOP 5 MEJORES TIEMPOS ---\n\n";

                         if (!snapshot.HasChildren)
                         {
                             textoFinal += "Aún no hay récords. ¡Sé el primero!";
                         }
                         else
                         {
                             int posicion = 1;

                             // Recorremos los datos que nos mandó Firebase
                             foreach (DataSnapshot hijo in snapshot.Children)
                             {
                                 // Extraemos las variables tal como las guardaste en tu RecordJugador
                                 string nombre = hijo.Child("nombreJugador").Value.ToString();
                                 float tiempo = float.Parse(hijo.Child("tiempoSegundos").Value.ToString());

                                 // Damos formato a la línea (Ej. "1. Jugador_1 - 83.91 s")
                                 textoFinal += posicion + ". " + nombre + " - " + tiempo.ToString("F2") + " s\n\n";
                                 posicion++;
                             }
                         }

                         // Escribimos el resultado final en la pantalla
                         if (textoMarcadores != null)
                         {
                             textoMarcadores.text = textoFinal;
                         }

                         Debug.Log("Marcadores descargados con éxito.");
                     }
                 });
    }
}