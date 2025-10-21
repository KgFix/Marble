using UnityEngine;
using UnityEngine.SceneManagement;

// Ensures GameState is reset on every scene load, even if a CheckpointManager is not present.
public static class GameStateInitializer
{
    // Reset immediately before a scene loads so HUD shows 00:00 on first frame
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PreSceneReset()
    {
        GameState.Reset(0);
        // Ensure we don't double-subscribe across reloads
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // After the scene loads, recalc checkpoint count and finalize reset
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var checkpoints = Object.FindObjectsOfType<Checkpoint>();
        int count = checkpoints != null ? checkpoints.Length : 0;
        GameState.Reset(count);
        // Keep subscribed for subsequent reloads
    }
}
