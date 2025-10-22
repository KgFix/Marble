using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CoinManager3 : MonoBehaviour
{
    public static CoinManager3 Instance { get; private set; }

    private List<Collectable> allCollectables = new List<Collectable>();
    private int collectedCount = 0;
    private int collectedValue = 0;

    // BossHealth is set to the number of collectables at Start
    public int BossHealth { get; private set; }

    [Header("UI References")]
    public GameObject endScreenPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI timeTakenText;
    public GameObject retryButton;
    public GameObject nextLevelButton;

    [Header("Player Reference")]
    public GameObject playerObject;

    [Header("Camera Reference")]
    public CameraTilt3 cameraTilt3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        allCollectables.Clear();
        allCollectables.AddRange(GetComponentsInChildren<Collectable>());
        collectedCount = 0;
        collectedValue = 0;
        BossHealth = allCollectables.Count;

        // Hide end screen at start
        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);

        // Ensure player is enabled at start
        if (playerObject != null)
            playerObject.SetActive(true);

        // Optionally auto-assign CameraTilt3 if not set
        if (cameraTilt3 == null)
            cameraTilt3 = FindObjectOfType<CameraTilt3>();
    }

    private void Update()
    {
        // Dev cheat: Hold '>' (period) and press 'L' to win instantly
        if (Input.GetKey(KeyCode.Period) && Input.GetKeyDown(KeyCode.L))
        {
            collectedCount = allCollectables.Count;

            if (!GameState.IsCompleted)
            {
                GameState.SetVictory(true);
                GameState.CompleteLevel();

                float winTime = GameState.LevelTime;
                var endScreen = FindObjectOfType<EndScreenUI3>(includeInactive: true);
                if (endScreen != null)
                    endScreen.ShowEndScreen(true, winTime);
                else if (!EndScreenHelper.TryShowEndScreen())
                    Debug.LogWarning("Win: No End Screen UI found. Ensure an end screen exists in the scene.");
            }
        }
    }

    public void Collect(Collectable collectable)
    {
        if (allCollectables.Contains(collectable))
        {
            collectedCount++;
            collectedValue += collectable.collectableValue;

            // WIN CONDITION: All coins collected
            if (collectedCount >= allCollectables.Count)
            {
                if (!GameState.IsCompleted)
                {
                    GameState.SetVictory(true);
                    GameState.CompleteLevel();

                    float winTime = GameState.LevelTime;
                    var endScreen = FindObjectOfType<EndScreenUI3>(includeInactive: true);
                    if (endScreen != null)
                        endScreen.ShowEndScreen(true, winTime);
                    else if (!EndScreenHelper.TryShowEndScreen())
                        Debug.LogWarning("Win: No End Screen UI found. Ensure an end screen exists in the scene.");
                }
            }
        }
    }

    public int TotalCollectables => allCollectables.Count;
    public int CollectedCount => collectedCount;
    public int CollectedValue => collectedValue;
}
