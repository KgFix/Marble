using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [Tooltip("Optional: assign a shared audio source. If empty the script will create one on the scene root.")]
    public AudioSource audioSource;

    [Tooltip("Sound to play when the button is clicked")]
    public AudioClip clickSound;

    [System.Obsolete]
    void Start()
    {
        // if script hasn't got an audio source assigned, try to find one in scene
        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
            if (audioSource != null)
                Debug.Log("[UIButtonSound] Found existing AudioSource: " + audioSource.gameObject.name);
        }

        // if still null, create a dedicated AudioSource on a GameObject called "UISoundPlayer"
        if (audioSource == null)
        {
            GameObject go = GameObject.Find("UISoundPlayer");
            if (go == null)
            {
                go = new GameObject("UISoundPlayer");
                DontDestroyOnLoad(go); // optional - keep across loads
            }

            audioSource = go.GetComponent<AudioSource>();
            if (audioSource == null) audioSource = go.AddComponent<AudioSource>();

            // default settings suitable for UI sounds:
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D sound
            audioSource.volume = 1f;
            Debug.Log("[UIButtonSound] Created UISoundPlayer with AudioSource.");
        }

        // ensure there's a clip assigned at least (warn if not)
        if (clickSound == null)
            Debug.LogWarning("[UIButtonSound] No clickSound assigned on " + gameObject.name + ". Assign an AudioClip in the inspector.");

        // hook into the Button
        Button btn = GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogError("[UIButtonSound] No Button component found on this GameObject.");
            return;
        }

        btn.onClick.AddListener(OnClick);
        Debug.Log("[UIButtonSound] Hooked button click for " + gameObject.name);
    }

    void OnClick()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("[UIButtonSound] No audioSource available to play sound.");
            return;
        }
        if (clickSound == null)
        {
            Debug.LogWarning("[UIButtonSound] No click sound assigned.");
            return;
        }

        audioSource.PlayOneShot(clickSound);
        Debug.Log("[UIButtonSound] Played click sound: " + clickSound.name);
    }

    // optional: cleanup
    void OnDestroy()
    {
        // remove listeners to avoid leaks while editing
        var btn = GetComponent<Button>();
        if (btn != null) btn.onClick.RemoveListener(OnClick);
    }
}

