using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Resolution Settings")]
    public int targetWidth = 1920;
    public int targetHeight = 1080;
    public bool forceFullscreen = false;
    public bool lockAspectRatio = true;

    void Awake()
    {
        // This runs before Start(), ensuring resolution is set early
        SetResolution();
        Time.fixedDeltaTime = 0.02f;

        // Optional: Make this persist across scenes
        DontDestroyOnLoad(gameObject);
    }

    void SetResolution()
    {
        if (lockAspectRatio)
        {
            // Force specific resolution
            Screen.SetResolution(targetWidth, targetHeight, forceFullscreen);
        }
        else
        {
            // Just set fullscreen mode
            Screen.fullScreen = forceFullscreen;
        }

        Debug.Log($"Resolution set to: {Screen.width}x{Screen.height}");
    }
}