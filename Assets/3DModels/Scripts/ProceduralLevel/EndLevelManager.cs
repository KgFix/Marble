using TMPro;
using UnityEngine;

public class EndLevelUIManager : MonoBehaviour
{
    public static EndLevelUIManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject endScreenPanel;
    public TextMeshProUGUI timeTakenText;
    public GameObject star1, star2, star3;

    [Header("Star Rating Settings")]
    [Tooltip("Time in seconds or less for 3 stars.")]
    public float threeStarTimeThreshold = 60f;
    [Tooltip("Time in seconds or less for 2 stars.")]
    public float twoStarTimeThreshold = 120f;

    private void Awake()
    {
        Instance = this;
        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);
    }

    public void ShowEndScreen(float levelTime)
    {
        if (endScreenPanel != null)
            endScreenPanel.SetActive(true);

        // Update timer text
        if (timeTakenText != null)
        {
            int minutes = Mathf.FloorToInt(levelTime / 60f);
            int seconds = Mathf.FloorToInt(levelTime % 60f);
            int hundredths = Mathf.FloorToInt((levelTime * 100f) % 100f);

            timeTakenText.text = string.Format("Time Taken: {0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);
 }

        // Update star rating
        int stars = CalculateStarRating(levelTime);
        if (star1 != null) star1.SetActive(stars >= 1);
        if (star2 != null) star2.SetActive(stars >= 2);
        if (star3 != null) star3.SetActive(stars == 3);
    }

    public int CalculateStarRating(float time)
    {
        if (time <= threeStarTimeThreshold)
            return 3;
        if (time <= twoStarTimeThreshold)
            return 2;
        return 1;
    }
}
