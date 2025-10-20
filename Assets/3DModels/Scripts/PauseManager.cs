using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Root panel for pause UI - should be inactive on scene start")]
    public GameObject pausePanel;

    [Header("Scenes")]
    [Tooltip("Scene name to load when Exit is pressed (e.g. LevelSelect or MainMenu)")]
    public string exitSceneName = "LevelMenu";

    [Tooltip("Scene name for Options. This will be loaded when Options is pressed.")]
    public string optionsSceneName = "OptionsMenu";

    [Header("Audio")]
    [Tooltip("If you want to mute audio on pause, assign true and optionally use AudioListener.pause")]
    public bool muteAudioOnPause = false;

    // internal
    bool isPaused = false;
    float previousTimeScale = 1f;

    private void Awake()
    {
        // singleton convenience
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // safety: ensure pause panel is hidden at runtime
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void Update()
    {
        // Toggle with Escape (or Q) — change as you like
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public bool IsPaused => isPaused;

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (isPaused) return;

        // show UI
        if (pausePanel != null) pausePanel.SetActive(true);

        // freeze time
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // optional audio mute
        if (muteAudioOnPause) AudioListener.pause = true;

        // show mouse cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        isPaused = true;
    }

    public void Resume()
    {
        if (!isPaused) return;

        // hide UI
        if (pausePanel != null) pausePanel.SetActive(false);

        // restore time
        Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;

        // restore audio
        if (muteAudioOnPause) AudioListener.pause = false;

        // restore cursor state for gameplay (change if your game uses different locking)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isPaused = false;
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        // restore timescale so things unload correctly
        Time.timeScale = 1f;
        if (muteAudioOnPause) AudioListener.pause = false;

        // optional: small frame to allow UI click SFX to play
        yield return null;

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
        yield break;
    }

    public void ExitToMenu()
    {
        StartCoroutine(ExitRoutine());
    }

    private IEnumerator ExitRoutine()
    {
        Time.timeScale = 1f;
        if (muteAudioOnPause) AudioListener.pause = false;

        // safety check: ensure scene is in build settings
        if (string.IsNullOrEmpty(exitSceneName))
        {
            Debug.LogError("[PauseController] exitSceneName is empty.");
            yield break;
        }

        bool found = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == exitSceneName) { found = true; break; }
        }

        if (!found)
        {
            Debug.LogError($"[PauseController] Scene '{exitSceneName}' not in Build Settings.");
            yield break;
        }

        // load scene
        var op = SceneManager.LoadSceneAsync(exitSceneName);
        while (!op.isDone) yield return null;
    }

    /// <summary>
    /// Open the Options scene. Saves the current scene as "PreviousScene" in PlayerPrefs
    /// so the options scene can return to it (LoadPreviousScene). Restores Time.timeScale/audio before loading.
    /// </summary>
    public void OpenOptionsScene()
    {
        StartCoroutine(OpenOptionsRoutine());
    }

    private IEnumerator OpenOptionsRoutine()
    {
        // restore timescale & audio so the options scene loads and plays audio/UI normally
        Time.timeScale = 1f;
        if (muteAudioOnPause) AudioListener.pause = false;

        if (string.IsNullOrEmpty(optionsSceneName))
        {
            Debug.LogError("[PauseController] optionsSceneName is empty.");
            yield break;
        }

        // Save current scene so the options scene can go back
        string current = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("PreviousScene", current);
        PlayerPrefs.Save();
        Debug.Log($"[PauseController] Saved previous scene '{current}' to PlayerPrefs (PreviousScene).");

        // check scene exists in build settings
        bool found = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == optionsSceneName) { found = true; break; }
        }
        if (!found)
        {
            Debug.LogError($"[PauseController] Options scene '{optionsSceneName}' not in Build Settings.");
            yield break;
        }

        // load options scene
        var op = SceneManager.LoadSceneAsync(optionsSceneName);
        while (!op.isDone) yield return null;
    }
}
