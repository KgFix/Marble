using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Options")]
    [Tooltip("If true, timer starts automatically on scene load. If false, must be started manually (e.g., after countdown).")]
    public bool autoStartOnSceneLoad = true;

    private float elapsedTime = 0f;
    private bool isRunning = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetTimer();

        if (autoStartOnSceneLoad)
            StartTimer();
    }

    private void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;

        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 1000) % 1000);

        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;

        if (timerText != null)
            timerText.text = "00:00.000";
    }
}
