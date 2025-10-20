using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("Main Mixer")]
    public AudioMixer mainMixer;

    [Header("UI Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Defaults")]
    [Range(0.0001f, 1f)]
    public float defaultVolume = 0.75f;

    // a small floor to avoid accidental instant-mute clicks (you can set to 0 to allow true silence)
    const float minLinear = 0.01f; // 1% - prevents accidental full mute when user clicks

    void Start()
    {
        // ensure mixer assigned
        if (mainMixer == null)
        {
            Debug.LogError("[AudioSettingsSafeInit] mainMixer not assigned!");
            return;
        }

        // initialize each slider safely
        SafeInitSlider(masterSlider, "MasterVolume", SetMasterVolume);
        SafeInitSlider(musicSlider, "MusicVolume", SetMusicVolume);
        SafeInitSlider(sfxSlider, "SFXVolume", SetSFXVolume);
    }

    void SafeInitSlider(Slider slider, string prefKey, UnityEngine.Events.UnityAction<float> handler)
    {
        if (slider == null) return;

        // ensure range + no whole numbers
        slider.wholeNumbers = false;
        slider.minValue = 0f;
        slider.maxValue = 1f;

        // remove inspector-assigned listeners so we don't get duplicates
        slider.onValueChanged.RemoveAllListeners();

        // load saved value or default
        float saved = PlayerPrefs.GetFloat(prefKey, defaultVolume);

        // apply value without invoking OnValueChanged to prevent accidental SetFloat calls during startup
        slider.SetValueWithoutNotify(saved);

        // now hook handler (only once)
        slider.onValueChanged.AddListener(handler);

        // call handler once to apply mixer state to match slider (but with protection)
        handler.Invoke(saved);

        Debug.Log($"[AudioSettingsSafeInit] Initialized {prefKey} = {saved:F3}");
    }

    // convert linear 0..1 into dB safely (0 => -80dB). We enforce a tiny floor to avoid accidental full-mute.
    float LinearToDb(float linear)
    {
        linear = Mathf.Clamp01(linear);

        // apply small floor to avoid accidental full-mute; if you want to allow true silence, set minLinear = 0f
        if (linear <= Mathf.Epsilon)
            linear = 0f;
        if (linear > 0f && linear < minLinear)
            linear = minLinear;

        return (linear <= 0.0001f) ? -80f : Mathf.Log10(linear) * 20f;
    }

    void SetVolumeInternal(string param, float linear)
    {
        float dB = LinearToDb(linear);
        bool ok = mainMixer.SetFloat(param, dB);
        Debug.Log($"[AudioSettingsSafeInit] {param} set: linear={linear:F3} -> dB={dB:F1}, ok={ok}");
        PlayerPrefs.SetFloat(param, linear);
    }

    // public handlers for sliders
    public void SetMasterVolume(float v) => SetVolumeInternal("MasterVolume", v);
    public void SetMusicVolume(float v) => SetVolumeInternal("MusicVolume", v);
    public void SetSFXVolume(float v) => SetVolumeInternal("SFXVolume", v);

    // debug helper
    [ContextMenu("DebugReadAllVolumes")]
    public void DebugReadAllVolumes()
    {
        string[] parameters = { "MasterVolume", "MusicVolume", "SFXVolume" };
        foreach (string p in parameters)
        {
            if (mainMixer.GetFloat(p, out float val))
                Debug.Log($"{p} = {val} dB");
            else
                Debug.LogWarning($"{p} not found or not exposed!");
        }
    }
}
