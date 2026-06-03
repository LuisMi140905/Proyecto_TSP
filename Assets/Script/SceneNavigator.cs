using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Android; // <-- Necesario para pedir permisos en Android

public sealed class SceneNavigator : MonoBehaviour
{
    private Coroutine loadSceneCoroutine;

    // Esto se ejecuta al instante en cuanto abres el juego
    void Start()
    {
        // Si el usuario no ha dado permiso de cámara...
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            // ...se lo pedimos con la ventanita típica de Android
            Permission.RequestUserPermission(Permission.Camera);
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("No se asignó el nombre de la escena.");
            return;
        }

        if (loadSceneCoroutine != null)
        {
            return;
        }

        loadSceneCoroutine = StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadOperation.isDone)
        {
            yield return null;
        }
    }

    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        LoadSceneByName(currentScene.name);
    }

    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}