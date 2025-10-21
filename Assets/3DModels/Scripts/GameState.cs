using UnityEngine;

public static class GameState
{
    public static bool IsCompleted { get; private set; } = false;
    public static bool InputEnabled { get; private set; } = false;
    public static bool IsVictory { get; private set; } = false;
    public static int CurrentCheckpointIndex { get; private set; } = 0;
    public static int TotalCheckpoints { get; private set; } = 0;
    public static float LevelTime { get; private set; } = 0f;
    private static bool timerRunning = false;

    public static void Reset(int checkpointCount)
    {
        IsCompleted = false;
        InputEnabled = false;
        IsVictory = false;
        CurrentCheckpointIndex = 0;
        TotalCheckpoints = checkpointCount;
        LevelTime = 0f;
        timerRunning = false;
        Debug.Log($"GameState reset: TotalCheckpoints={TotalCheckpoints}, CurrentCheckpointIndex={CurrentCheckpointIndex}");
    }

    public static void StartGame()
    {
        InputEnabled = true;
        timerRunning = true;
    }

    public static void UpdateTimer(float deltaTime)
    {
        if (timerRunning)
            LevelTime += deltaTime;
    }

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

    public static void SetVictory(bool victory)
    {
        IsVictory = victory;
    }
}
