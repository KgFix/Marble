using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Drop this on your End Screen root (or any object in the Canvas) and
// wire its fields in the Inspector. Hook your Button OnClick to the
// public methods OnRetryButton / OnExitButton.
public class GameEndScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private TextMeshProUGUI timeTakenText;
    [SerializeField] private TextMeshProUGUI collectablesText;
    [SerializeField] private TextMeshProUGUI titleText; // Completed/Failed title

    [Header("Title Settings")]
    [SerializeField] private string victoryTitle = "COMPLETED!";
    [SerializeField] private string failureTitle = "FAILED";
    [Tooltip("Optional: Drag a whole GameObject (e.g., a header group) to show on victory")]
    [SerializeField] private GameObject victoryObject;
    [Tooltip("Optional: Drag a whole GameObject (e.g., a header group) to show on failure")]
    [SerializeField] private GameObject failureObject;
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;
    [Tooltip("Optional: Parent of all stars to hide entirely on failure")] [SerializeField] private GameObject starsParent;

    [Header("Buttons (Optional)")]
    [Tooltip("Drag your Retry button here to auto-wire OnClick")] [SerializeField] private Button retryButton;
    [Tooltip("Drag your Exit button here to auto-wire OnClick")] [SerializeField] private Button exitButton;

    [Header("Visibility Helpers (Optional)")]
    [Tooltip("If set, this Canvas will be forced to a high sorting order when the end screen shows")]
    [SerializeField] private Canvas canvasToBringFront;
    [SerializeField] private int bringFrontSortingOrder = 100;
    [Tooltip("Panels to disable when end screen shows (e.g., Pause menu)")]
    [SerializeField] private GameObject[] panelsToHideOnShow;

    [Header("Star Rating Settings")]
    [SerializeField] private float threeStarTimeThreshold = 60f;

    private void Awake()
    {
        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);
        else
            Debug.LogWarning("GameEndScreen: 'endScreenPanel' is not assigned. End screen cannot be shown.");

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

    public void ShowEndScreen()
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
            endScreenPanel.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogWarning("GameEndScreen: Cannot show because 'endScreenPanel' is missing.");
            return;
        }

        // Title based on victory/failure
        bool isVictory = GameState.IsVictory;
        if (victoryObject != null)
            victoryObject.SetActive(isVictory);
        if (failureObject != null)
            failureObject.SetActive(!isVictory);

        if (titleText != null)
        {
            titleText.gameObject.SetActive(true);
            titleText.text = isVictory ? victoryTitle : failureTitle;
        }

        // Optionally boost sorting order for the end-screen Canvas
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

        // Update time text
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

        // Stars
        int stars = CalculateStarRating();
        if (starsParent != null)
            starsParent.SetActive(stars > 0);
        if (star1 != null) star1.SetActive(stars >= 1);
        if (star2 != null) star2.SetActive(stars >= 2);
        if (star3 != null) star3.SetActive(stars == 3);
    }

    private int CalculateStarRating()
    {
        // New rule:
        // - On failure: if collected >= 3, guarantee 1 star; else 0.
        // - On victory: 3 stars if all collectables and under time; 2 stars if all collectables; else 1 star.
        int collected = 0;
        int total = 0;
        if (CollectableManager.Instance != null)
        {
            collected = CollectableManager.Instance.CollectedCount;
            total = CollectableManager.Instance.TotalCollectables;
        }

        if (!GameState.IsVictory)
        {
            bool allCollectablesFail = total > 0 && collected >= total;
            if (allCollectablesFail) return 2; // new: 2 stars on failure if all collectables
            return collected >= 3 ? 1 : 0;
        }

        bool allCollectables = total > 0 && collected >= total;
        bool underTime = GameState.LevelTime <= threeStarTimeThreshold;

        if (allCollectables && underTime) return 3;
        if (allCollectables) return 2;
        if (collected >= 3) return 2; // new rule: at least 3 collectables guarantees 2 stars on victory
        return 1;
    }

    // Hook this to your Retry button's OnClick
    public void OnRetryButton()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Hook this to your Exit button's OnClick
    public void OnExitButton()
    {
        SceneManager.LoadScene("LevelMenu");
    }
}
