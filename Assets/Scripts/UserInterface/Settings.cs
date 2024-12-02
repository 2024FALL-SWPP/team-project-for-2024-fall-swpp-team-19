using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
   [Header("UI Sliders")]
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public Slider sensitivitySlider;

    public Image brightnessOverlay; 

    void Start()
    {
        // Set initial values
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 0.5f);
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 0.5f);

        // Add listeners
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
        brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
    }

    private void UpdateVolume(float value)
    {
        // Save volume value
        PlayerPrefs.SetFloat("Volume", value);
        Debug.Log($"Volume set to: {value}");
    }

    private void UpdateBrightness(float value)
    {
        PlayerPrefs.SetFloat("Brightness", value);

        // Adjust the alpha channel of the overlay
        Color color = brightnessOverlay.color;
        color.a = 1.0f - value; // value가 높을수록 더 밝게
        brightnessOverlay.color = color;

        Debug.Log($"Brightness set to: {value}");
    }

    private void UpdateSensitivity(float value)
    {
        // Save sensitivity value
        PlayerPrefs.SetFloat("Sensitivity", value);
        Debug.Log($"Sensitivity set to: {value}");
    }

    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        volumeSlider.onValueChanged.RemoveAllListeners();
        brightnessSlider.onValueChanged.RemoveAllListeners();
        sensitivitySlider.onValueChanged.RemoveAllListeners();
    }

    public void Close(){
        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay(){
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
