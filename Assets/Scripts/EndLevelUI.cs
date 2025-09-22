using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private TextMeshProUGUI timeTakenText;
    [SerializeField] private TextMeshProUGUI collectablesText;

    private void Awake()
    {
        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);
    }

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
        if (collectablesText != null && CollectableManager.Instance != null)
        {
            collectablesText.text = $"Collectables Found: {CollectableManager.Instance.CollectedCount}/{CollectableManager.Instance.TotalCollectables}";
        }
    }

    public void OnRetryButton()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void OnExitButton()
    {
        SceneManager.LoadScene("LevelMenu");
    }
}
