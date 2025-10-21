using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Level 2 End Screen: copied from Level 1 but ONLY time-based stars (no collectables)
public class EndGameScreenforlevel2 : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private TextMeshProUGUI timeTakenText;
    [SerializeField] private TextMeshProUGUI titleText; // Completed/Failed title (optional)

    [Header("Title Settings")]
    [SerializeField] private string victoryTitle = "COMPLETED!";
    [SerializeField] private string failureTitle = "FAILED";
    [Tooltip("Optional: Drag a whole GameObject (e.g., a header group) to show on victory")]
    [SerializeField] private GameObject victoryObject;
    [Tooltip("Optional: Drag a whole GameObject (e.g., a header group) to show on failure")]
    [SerializeField] private GameObject failureObject;

    [Header("Stars")]
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;
    [Tooltip("Optional: Parent of all stars to hide entirely on failure")] 
    [SerializeField] private GameObject starsParent;

    [Header("Buttons (Optional)")]
    [Tooltip("Drag your Retry button here to auto-wire OnClick")] 
    [SerializeField] private Button retryButton;
    [Tooltip("Drag your Exit button here to auto-wire OnClick")] 
    [SerializeField] private Button exitButton;

    [Header("Visibility Helpers (Optional)")]
    [Tooltip("If set, this Canvas will be forced to a high sorting order when the end screen shows")]
    [SerializeField] private Canvas canvasToBringFront;
    [SerializeField] private int bringFrontSortingOrder = 100;
    [Tooltip("Panels to disable when end screen shows (e.g., Pause menu)")]
    [SerializeField] private GameObject[] panelsToHideOnShow;

    [Header("Time thresholds (seconds) for stars")]
    [Tooltip("Time (sec) to achieve 3 stars: if LevelTime <= this, you get 3 stars")]
    [Min(0f)]
    [SerializeField] private float threeStarTime = 60f;
    [Tooltip("Time (sec) to achieve 2 stars (used when time > 3-star but <= 2-star)")]
    [Min(0f)]
    [SerializeField] private float twoStarTime = 90f;
    [Tooltip("Time (sec) to achieve 1 star (used when time > 2-star but <= 1-star)")]
    [Min(0f)]
    [SerializeField] private float oneStarTime = 120f;

    private void Awake()
    {
        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);
        else
            Debug.LogWarning("EndGameScreenforlevel2: 'endScreenPanel' is not assigned. End screen cannot be shown.");

        // Auto-wire buttons if provided
        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(OnRetryButton);
            retryButton.onClick.AddListener(OnRetryButton);
        }
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButton);
            exitButton.onClick.AddListener(OnExitButton);
        }
    }

    private void OnValidate()
    {
        // Keep thresholds ordered: three <= two <= one
        if (twoStarTime < threeStarTime) twoStarTime = threeStarTime;
        if (oneStarTime < twoStarTime) oneStarTime = twoStarTime;
    }

    // Call this to show the end screen using GameState.LevelTime
    public void ShowEndScreen()
    {
        ShowEndScreen(GameState.LevelTime, GameState.IsVictory);
    }

    // Overload: call with explicit time and outcome
    public void ShowEndScreen(float timeSeconds, bool isVictory)
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
            endScreenPanel.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogWarning("EndGameScreenforlevel2: Cannot show because 'endScreenPanel' is missing.");
            return;
        }

        // Title based on victory/failure
        if (victoryObject != null) victoryObject.SetActive(isVictory);
        if (failureObject != null) failureObject.SetActive(!isVictory);
        if (titleText != null)
        {
            titleText.gameObject.SetActive(true);
            titleText.text = isVictory ? victoryTitle : failureTitle;
        }

        // Bring this Canvas to front if requested
        if (canvasToBringFront != null)
        {
            canvasToBringFront.overrideSorting = true;
            canvasToBringFront.sortingOrder = bringFrontSortingOrder;
        }

        // Optionally hide overlapping panels (e.g., Pause)
        if (panelsToHideOnShow != null)
        {
            foreach (var go in panelsToHideOnShow)
                if (go != null) go.SetActive(false);
        }

        // Update time text only (no collectables on Level 2)
        if (timeTakenText != null)
        {
            float t = timeSeconds;
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);
            int milliseconds = Mathf.FloorToInt((t * 1000f) % 1000f);
            timeTakenText.text = string.Format("Time Taken: {0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }

        // Stars are based ONLY on time, and only when victorious
        int stars = isVictory ? CalculateStarsFromTime(timeSeconds) : 0;
        if (starsParent != null) starsParent.SetActive(stars > 0);
        if (star1 != null) star1.SetActive(stars >= 1);
        if (star2 != null) star2.SetActive(stars >= 2);
        if (star3 != null) star3.SetActive(stars >= 3);
    }

    private int CalculateStarsFromTime(float timeSeconds)
    {
        if (timeSeconds <= threeStarTime) return 3;
        if (timeSeconds <= twoStarTime) return 2;
        if (timeSeconds <= oneStarTime) return 1;
        return 0;
    }

    // Hook to Retry button
    public void OnRetryButton()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Hook to Exit button
    public void OnExitButton()
    {
        SceneManager.LoadScene("LevelMenu");
    }
}
