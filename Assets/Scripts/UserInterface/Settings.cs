using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class Settings : MonoBehaviour
{
   [Header("UI Sliders")]

    [Header("Post Processing")]
    public Volume postProcessingVolume; // Post Processing Volume

    private ColorAdjustments colorAdjustments;
   
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public Slider sensitivitySlider;

 

    void Start()
    {

         if (postProcessingVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0.5f);
            UpdateBrightness(savedBrightness); // 초기값 적용
        }
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

        if (colorAdjustments != null)
        {
            // Exposure 값 설정 (밝기 조정)
            colorAdjustments.postExposure.value = Mathf.Lerp(-2f, 2f, value); // -2에서 2 사이 값으로 조정
        }

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
