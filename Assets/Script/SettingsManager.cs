using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider volumeSlider;
    public Toggle comfortModeToggle;

    void Start()
    {
        // Cargar valores guardados; si es la primera vez, el volumen será 1 y el confort falso
        volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1.0f);

        int comfortValue = PlayerPrefs.GetInt("ComfortMode", 0);
        comfortModeToggle.isOn = (comfortValue == 1);

        AudioListener.volume = volumeSlider.value;
    }

    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void ToggleComfortMode(bool isOn)
    {
        PlayerPrefs.SetInt("ComfortMode", isOn ? 1 : 0);
    }

    public void SaveAndExit()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene("MenúInicio");
    }
}
