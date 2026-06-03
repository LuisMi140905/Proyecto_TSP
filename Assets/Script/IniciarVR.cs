using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

public class IniciarVR : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ArrancarVRSeguro());
    }

    IEnumerator ArrancarVRSeguro()
    {
        // Esperamos medio segundo a que el teléfono estabilice la aplicación
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Iniciando XR Management...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Falló la inicialización de XR.");
        }
        else
        {
            Debug.Log("XR Inicializado. Arrancando subsistemas...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
        }
    }
}