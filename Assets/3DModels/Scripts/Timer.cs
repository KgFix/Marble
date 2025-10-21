using UnityEngine;
using TMPro;
using System.Collections;

// Per-checkpoint countdown timer
// Shows a UI countdown for the NEXT checkpoint to reach. If time hits 0 before
// the player activates that checkpoint, the player fails (similar flow to Deadzone).
public class Timer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI checkpointTimerText; // Assign in Inspector
    [Tooltip("Hide the timer text once all checkpoints are completed (and no final segment is active)")]
    [SerializeField] private bool hideWhenAllCheckpointsDone = true;

    [Header("Display")]
    [Tooltip("Delay (seconds) before showing the timer UI at level start")]
    [SerializeField] private float initialDisplayDelay = 5f;

    [Header("Timing")]
    [Tooltip("Default time (seconds) for each checkpoint segment if not overridden")]
    [SerializeField] private float defaultCheckpointTime = 15f;
    [Tooltip("Optional: per-checkpoint custom times. Index = checkpoint index expected next. If index is out of range or <= 0, default is used.")]
    [SerializeField] private float[] perCheckpointTimes;

    [Tooltip("After the last checkpoint, also start a final segment to reach the Finish Line")]
    [SerializeField] private bool includeFinalFinishSegment = true;
    [Tooltip("Time (seconds) for the final segment from the last checkpoint to the Finish Line. If <= 0, uses Default Checkpoint Time")]
    [SerializeField] private float finalSegmentTime = 15f;

    [Header("On Timeout (Fail)"), Tooltip("Shown and used when timer runs out before reaching the next checkpoint")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private MonoBehaviour[] cameraScripts;
    [SerializeField] private TextMeshProUGUI completionText; // Reuse End text slot to show failure message
    [SerializeField] private EndScreenUI endScreenUI;
    [SerializeField] private string timeoutMessage = "Time's Up!";

    private float timeLeft = 0f;
    private int trackingNextCheckpointIndex = 0; // The checkpoint we are timing towards
    private bool timerActive = false;
    private bool failed = false;
    private bool onFinalSegment = false; // true when timing the run to the Finish Line
    private bool canDisplayUI = false; // gates showing the UI until delay passes

    void Start()
    {
        // Auto-wire common refs if forgotten in Inspector
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (endScreenUI == null)
            endScreenUI = SceneUtil.FindInScene<EndScreenUI>(includeInactive: true);
        if (endScreenUI == null)
            Debug.LogWarning("Timer: EndScreenUI reference not set and not found in scene. End screen may not show on timeout.");
        if (mainCamera == null)
            Debug.LogWarning("Timer: Main Camera reference not set and Camera.main not found. Camera freeze on timeout will be skipped.");

        // Initialize tracking to whatever the game currently expects next
        trackingNextCheckpointIndex = GameState.CurrentCheckpointIndex;
        onFinalSegment = false;
        TryStartOrResetSegment();
        UpdateDisplay();

        // Start delayed display for the timer UI
        if (checkpointTimerText != null)
        {
            checkpointTimerText.gameObject.SetActive(false);
        }
        StartCoroutine(EnableDisplayAfterDelay());
    }

    void Update()
    {
        if (failed) return;

        // If the level has completed, stop and hide the timer UI
        if (GameState.IsCompleted)
        {
            timerActive = false;
            if (checkpointTimerText != null)
                checkpointTimerText.gameObject.SetActive(false);
            return;
        }

        // Don't count down if inputs not enabled yet (pre-countdown/paused state)
        if (!GameState.InputEnabled)
        {
            return;
        }

        // If a checkpoint was activated (index increased), reset the segment timer for the next one
        if (GameState.CurrentCheckpointIndex != trackingNextCheckpointIndex)
        {
            trackingNextCheckpointIndex = GameState.CurrentCheckpointIndex;
            TryStartOrResetSegment();
        }

        if (!timerActive)
            return;

        // Countdown respecting timeScale (Time.deltaTime will be 0 while paused)
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;

        UpdateDisplay();

        if (timeLeft <= 0f)
        {
            HandleFailure();
        }
    }

    private IEnumerator EnableDisplayAfterDelay()
    {
        // Use realtime in case timescale changes during countdown
        float wait = Mathf.Max(0f, initialDisplayDelay);
        if (wait > 0f)
            yield return new WaitForSecondsRealtime(wait);
        canDisplayUI = true;
        RefreshUIVisibility();
    }

    private void RefreshUIVisibility()
    {
        if (checkpointTimerText == null) return;
        checkpointTimerText.gameObject.SetActive(canDisplayUI && timerActive);
    }

    private void TryStartOrResetSegment()
    {
        // If all checkpoints are already done, either start final segment or stop and optionally hide UI
        if (GameState.CurrentCheckpointIndex >= GameState.TotalCheckpoints)
        {
            if (includeFinalFinishSegment && !onFinalSegment && !GameState.IsCompleted)
            {
                StartFinalSegment();
            }
            else
            {
                timerActive = false;
                if (checkpointTimerText != null && hideWhenAllCheckpointsDone)
                    checkpointTimerText.gameObject.SetActive(false);
            }
            return;
        }

        onFinalSegment = false;
        timeLeft = GetTimeForSegment(GameState.CurrentCheckpointIndex);
        timerActive = true;
        RefreshUIVisibility();
    }

    private float GetTimeForSegment(int checkpointIndex)
    {
        float segment = defaultCheckpointTime;
        if (perCheckpointTimes != null && perCheckpointTimes.Length > 0)
        {
            int i = Mathf.Clamp(checkpointIndex, 0, perCheckpointTimes.Length - 1);
            float custom = perCheckpointTimes[i];
            if (custom > 0f) segment = custom;
        }
        return Mathf.Max(0.01f, segment); // avoid zero
    }

    private void StartFinalSegment()
    {
        onFinalSegment = true;
        timeLeft = GetTimeForFinalSegment();
        timerActive = true;
        RefreshUIVisibility();
        UpdateDisplay();
    }

    private float GetTimeForFinalSegment()
    {
        float segment = finalSegmentTime > 0f ? finalSegmentTime : defaultCheckpointTime;
        return Mathf.Max(0.01f, segment);
    }

    private void UpdateDisplay()
    {
        if (checkpointTimerText == null) return;

        // Simple seconds display, ceiling to show 3,2,1 distinctly
        int seconds = Mathf.CeilToInt(timeLeft);
        if (seconds < 0) seconds = 0;
        checkpointTimerText.text = seconds.ToString();
    }

    private void HandleFailure()
    {
        if (failed) return;
        if (GameState.IsCompleted) return; // in case another end condition already fired
        failed = true;
        timerActive = false;

    // Mark level as ended to stop inputs/timers
    GameState.SetVictory(false);
        GameState.CompleteLevel();

        // Freeze camera similarly to Deadzone
        if (mainCamera != null)
        {
            Vector3 camPos = mainCamera.transform.position;
            Quaternion camRot = mainCamera.transform.rotation;

            if (cameraScripts != null)
            {
                foreach (var script in cameraScripts)
                    if (script != null) script.enabled = false;
            }

            mainCamera.transform.SetPositionAndRotation(camPos, camRot);
        }

        // Show failure text
        if (completionText != null)
        {
            completionText.text = string.IsNullOrEmpty(timeoutMessage) ? "Time's Up!" : timeoutMessage;
            completionText.gameObject.SetActive(true);
        }

        // Show Level 2 end screen if present; otherwise fallback to Level 1 paths
        var end2 = SceneUtil.FindInScene<EndGameScreenforlevel2>(includeInactive: true);
        if (end2 != null)
        {
            end2.ShowEndScreen(GameState.LevelTime, false);
        }
        else if (endScreenUI != null)
        {
            endScreenUI.ShowEndScreen();
        }
        else if (!EndScreenHelper.TryShowEndScreen())
        {
            Debug.LogWarning("Timer: Cannot show end screen because EndScreenUI/GameEndScreen is missing.");
        }
    }
}
