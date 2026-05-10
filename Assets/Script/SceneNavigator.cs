using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneNavigator : MonoBehaviour
{
    private Coroutine loadSceneCoroutine;

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