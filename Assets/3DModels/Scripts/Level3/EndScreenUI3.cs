using TMPro;
using UnityEngine;

public class EndScreenUI3 : MonoBehaviour
{
    public static EndScreenUI3 Instance { get; private set; }

    [Header("UI References")]
    public GameObject endScreenPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI timeTakenText;
    public GameObject retryButton;
    public GameObject nextLevelButton;

    [Header("Player Reference")]
    [Tooltip("Assign the player GameObject here to disable it on end screen.")]
    public GameObject playerObject;

    [Header("Camera Reference")]
    [Tooltip("Assign the CameraTilt3 component here to disable it on end screen.")]
    public CameraTilt3 cameraTilt3;

    private void Awake()
    {
        Instance = this;
        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);

        // Ensure player is enabled at start
        if (playerObject != null)
            playerObject.SetActive(true);

        // Optionally, auto-assign CameraTilt3 if not set in inspector
        if (cameraTilt3 == null)
            cameraTilt3 = FindObjectOfType<CameraTilt3>();
    }

    public void ShowEndScreen(bool isVictory, float levelTime)
    {
        if (endScreenPanel != null)
            endScreenPanel.SetActive(true);

        if (resultText != null)
            resultText.text = isVictory ? "You Win!" : "You Failed!";

        if (timeTakenText != null)
        {
            int minutes = Mathf.FloorToInt(levelTime / 60f);
            int seconds = Mathf.FloorToInt(levelTime % 60f);
            int hundredths = Mathf.FloorToInt((levelTime * 100f) % 100f);
            timeTakenText.text = string.Format("Time Taken: {0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);
        }

        if (retryButton != null)
            retryButton.SetActive(!isVictory);

        if (nextLevelButton != null)
            nextLevelButton.SetActive(isVictory);

        // Disable the player object when showing the end screen
        if (playerObject != null)
            playerObject.SetActive(false);

        // Disable the CameraTilt3 script when showing the end screen
        if (cameraTilt3 != null)
            cameraTilt3.enabled = false;
    }
}
