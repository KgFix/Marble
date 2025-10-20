using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer mainMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Range(0.0001f, 1f)]
    public float defaultVolume = 0.75f; // default when no saved value

    void Awake()
    {
        // Ensure sliders exist and have proper ranges
        SetupSlider(masterSlider);
        SetupSlider(musicSlider);
        SetupSlider(sfxSlider);

        // Initialize slider values from PlayerPrefs (or defaults)
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", defaultVolume);

        // Apply the initial volumes
        SetMasterVolume(masterSlider.value);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);

        // Optionally hook listeners here if you prefer code-based wiring:
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    void SetupSlider(Slider s)
    {
        if (s == null) return;
        s.wholeNumbers = false;
        s.minValue = 0f;
        s.maxValue = 1f;
    }

    // Uses LERP mapping from linear 0..1 to sensible dB range (-80 to 0)
    private void SetVolumeLinear(string exposedParam, float linear01)
    {
        linear01 = Mathf.Clamp(linear01, 0f, 1f);
        // map 0..1 to -80..0 dB (linear slider -> dB)
        float dB = Mathf.Lerp(-80f, 0f, linear01);
        bool ok = mainMixer.SetFloat(exposedParam, dB);
        if (!ok) Debug.LogWarning($"Failed to set mixer param {exposedParam}");
    }

    public void SetMasterVolume(float val) { SetVolumeLinear("MasterVolume", val); PlayerPrefs.SetFloat("MasterVolume", val); }
    public void SetMusicVolume(float val) { SetVolumeLinear("MusicVolume", val); PlayerPrefs.SetFloat("MusicVolume", val); }
    public void SetSFXVolume(float val) { SetVolumeLinear("SFXVolume", val); PlayerPrefs.SetFloat("SFXVolume", val); }
}
