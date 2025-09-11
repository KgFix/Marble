using UnityEngine;

public static class GameState
{
    // ✅ Level completion
    public static bool IsCompleted { get; private set; } = false;

    // ✅ Input control (e.g., countdown or after finishing)
    public static bool InputEnabled { get; private set; } = false;

    // ✅ Checkpoint progress
    public static int CurrentCheckpointIndex { get; private set; } = 0;
    public static int TotalCheckpoints { get; private set; } = 0;

    // ✅ Level timer
    public static float LevelTime { get; private set; } = 0f;
    private static bool timerRunning = false;

    // --- METHODS ---

    // Reset for new level
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

    // Called by checkpoints
    public static void ActivateCheckpoint()
    {
        if (CurrentCheckpointIndex < TotalCheckpoints)
        {
            CurrentCheckpointIndex++;
            Debug.Log($"Checkpoint activated. Progress: {CurrentCheckpointIndex}/{TotalCheckpoints}");
        }
    }

    // Called by finish line
    public static void CompleteLevel()
    {
        IsCompleted = true;
        InputEnabled = false;
        timerRunning = false;
        Debug.Log($"Level Completed! Time: {LevelTime:F3} seconds");
    }
}
