using UnityEngine;

public static class GameState
{
    // --- Level completion ---
    public static bool IsCompleted { get; private set; } = false;

    // --- Input control (countdown, after finishing, etc.) ---
    public static bool InputEnabled { get; private set; } = false;

    // --- Checkpoints ---
    public static int CurrentCheckpointIndex { get; private set; } = 0;
    public static int TotalCheckpoints { get; private set; } = 0;

    // --- Level timer ---
    public static float LevelTime { get; private set; } = 0f;
    private static bool timerRunning = false;

    // --- Collectables ---
    public static int CollectedCount { get; private set; } = 0;
    public static int TotalCollectables { get; private set; } = 0;

    // ---------------- METHODS ----------------

    // Reset the level (checkpoints, timer, collectables)
    public static void Reset(int checkpointCount)
    {
        IsCompleted = false;
        InputEnabled = false;
        CurrentCheckpointIndex = 0;
        TotalCheckpoints = checkpointCount;
        LevelTime = 0f;
        timerRunning = false;

        Debug.Log($"GameState reset: TotalCheckpoints={TotalCheckpoints}, CurrentCheckpointIndex={CurrentCheckpointIndex}");
    }

    // Start input and timer (e.g., after countdown)
    public static void StartGame()
    {
        InputEnabled = true;
        timerRunning = true;
    }

    // Called every FixedUpdate or Update
    public static void UpdateTimer(float deltaTime)
    {
        if (timerRunning)
            LevelTime += deltaTime;
    }

    // --- Checkpoint methods ---
    public static void ActivateCheckpoint()
    {
        if (CurrentCheckpointIndex < TotalCheckpoints)
        {
            CurrentCheckpointIndex++;
            Debug.Log($"Checkpoint activated. Progress: {CurrentCheckpointIndex}/{TotalCheckpoints}");
        }
    }

    public static void CompleteLevel()
    {
        IsCompleted = true;
        InputEnabled = false;
        timerRunning = false;
        Debug.Log($"Level Completed! Time: {LevelTime:F3} seconds");
    }

    // --- Collectable methods ---
    public static void ResetCollectables(int total)
    {
        TotalCollectables = total;
        CollectedCount = 0;
        Debug.Log($"Collectables reset: {CollectedCount}/{TotalCollectables}");
    }

    public static void CollectItem()
    {
        CollectedCount++;
        Debug.Log($"Collectable collected! {CollectedCount}/{TotalCollectables}");
    }
}
