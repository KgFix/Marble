using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject endScreenPanel;      // Drag your panel here
    [SerializeField] private TextMeshProUGUI timeTakenText;  // Drag TextMeshPro for time
    [SerializeField] private TextMeshProUGUI collectablesText; // Drag TextMeshPro for collectables

    private void Awake()
    {
        if (endScreenPanel != null)
            endScreenPanel.SetActive(false); // Hide panel initially
    }

    // Call this method when the level ends
    public void ShowEndScreen()
    {
        if (endScreenPanel != null)
            endScreenPanel.SetActive(true);

        // Update timer text
        if (timeTakenText != null)
        {
            float t = GameState.LevelTime;
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);
            int milliseconds = Mathf.FloorToInt((t * 1000f) % 1000f);

            timeTakenText.text = string.Format("Time Taken: {0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }

        // Update collectables text
        if (collectablesText != null)
        {
            collectablesText.text = $"Collectables Found: {GameState.CollectedCount}/{GameState.TotalCollectables}";

        }


    }

    // Called by Retry button
    public void OnRetryButton()
    {
        // Reload the current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Called by Exit button
    public void OnExitButton()
    {
        // Load the LevelMenu scene
        SceneManager.LoadScene("LevelMenu");
    }
}
