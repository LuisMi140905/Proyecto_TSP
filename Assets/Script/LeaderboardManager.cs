using UnityEngine;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI Marcadores")]
    public TextMeshProUGUI textoMarcadores;

    private DatabaseReference reference;

    void Start()
    {
        if (textoMarcadores != null)
        {
            textoMarcadores.text = "Conectando con la base de datos...\nCargando tiempos...";
        }

        // AQUÍ ESTÁ LA MAGIA: Verificamos dependencias ANTES de llamar a DefaultInstance
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Ahora sí, Firebase está listo y no va a crashear tu celular
                reference = FirebaseDatabase.DefaultInstance.RootReference;
                DescargarMejoresTiempos();
            }
            else
            {
                Debug.LogError(System.String.Format("No se pudieron resolver las dependencias: {0}", dependencyStatus));
                if (textoMarcadores != null) textoMarcadores.text = "Error de conexión con el servidor.";
            }
        });
    }

    public void DescargarMejoresTiempos()
    {
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
                         string textoFinal = "--- TOP 5 MEJORES TIEMPOS ---\n\n";

                         if (!snapshot.HasChildren)
                         {
                             textoFinal += "Aún no hay récords. ¡Sé el primero!";
                         }
                         else
                         {
                             int posicion = 1;
                             foreach (DataSnapshot hijo in snapshot.Children)
                             {
                                 string nombre = hijo.Child("nombreJugador").Value.ToString();
                                 float tiempo = float.Parse(hijo.Child("tiempoSegundos").Value.ToString());
                                 textoFinal += posicion + ". " + nombre + " - " + tiempo.ToString("F2") + " s\n\n";
                                 posicion++;
                             }
                         }

                         if (textoMarcadores != null)
                         {
                             textoMarcadores.text = textoFinal;
                         }
                     }
                 });
    }
}