using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private TextMeshProUGUI timeTakenText;
    [SerializeField] private TextMeshProUGUI collectablesText;
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;

    [Header("Star Rating Settings")]
    [SerializeField] private float threeStarTimeThreshold = 60f; // Set your desired time in seconds

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

        // Update star rating
        int stars = CalculateStarRating();
        if (star1 != null) star1.SetActive(stars >= 1);
        if (star2 != null) star2.SetActive(stars >= 2);
        if (star3 != null) star3.SetActive(stars == 3);
    }

    private int CalculateStarRating()
    {
        // 1 star: level complete (always true if this UI is shown)
        // 2 stars: all collectables
        // 3 stars: all collectables + under time
        if (CollectableManager.Instance == null)
            return 1;

        bool allCollectables = CollectableManager.Instance.CollectedCount >= CollectableManager.Instance.TotalCollectables;
        bool underTime = GameState.LevelTime <= threeStarTimeThreshold;

        if (allCollectables && underTime)
            return 3;
        if (allCollectables)
            return 2;
        return 1;
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
