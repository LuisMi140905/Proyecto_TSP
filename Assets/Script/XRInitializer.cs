using UnityEngine;
using UnityEngine.XR.Management;

public class ArrancarVR : MonoBehaviour
{
    void Start()
    {
        // Esto inicializa la VR de forma asíncrona, 
        // evitando el choque de memoria en el arranque
        StartCoroutine(XRGeneralSettings.Instance.Manager.InitializeLoader());
        XRGeneralSettings.Instance.Manager.StartSubsystems();
    }
}